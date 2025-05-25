using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using AkinatorApi.Models;
using System.Security.Claims; // Для ClaimTypes
using System.Text.Json;      // Для JsonSerializer
using AkinatorApi.Data;      // Для AkinatorDbContextdo
using FSharpAnalytics;
using Microsoft.Extensions.Configuration;

namespace AkinatorApi.Controllers
{
    public enum GameState
    {
        AskingQuestions,
        AddingGameQuestions,
        WaitingForName
    }

    public class SessionData
    {
        public List<string> AnswersGiven { get; set; } = new();
        public List<string> AnswersToAdd { get; set; } = new();
        public GameState State { get; set; } = GameState.AskingQuestions;
    }

    [ApiController]
    [Route("api/[controller]")]
    public class AkinatorController : ControllerBase
    {

        private static ConcurrentDictionary<string, SessionData> Sessions = new();
        private readonly AkinatorDbContext _context; // Добавьте поле для контекста
        private readonly string _connectionString = "Host=localhost;Port=5432;Database=Akinator;Username=postgres;Password=12345";

        // Добавьте конструктор с внедрением зависимостей
        public AkinatorController(AkinatorDbContext context)
        {
            _context = context;
        }
        private string RunProlog(string args, string workingDir)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "swipl",
                Arguments = args,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = workingDir
            };

            using var process = Process.Start(psi);
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return output.Trim();
        }
        [HttpGet("top-active-users")]
        public IActionResult GetTopActiveUsers()
        {
            var rawResult = Queries.getTopActiveUsers(_connectionString);
            var result = rawResult.Select(tuple => new
            {
                username = tuple.Item1,
                session_count = tuple.Item2
            }).ToList();

            return Ok(result);
        }

        [HttpGet("users-with-characters-count")]
        public IActionResult GetUsersWithCharacterCounts()
        {
            var rawResult = Queries.getUsersWithCharacterCounts(_connectionString);
            var result = rawResult.Select(tuple => new
            {
                user_id = tuple.Item1,
                username = tuple.Item2,
                characters_added = tuple.Item3
            }).ToList();

            return Ok(result);
        }

        [HttpGet("users-with-questions-count")]
        public IActionResult GetUsersWithQuestionsCount()
        {
            var result = Queries.getUsersWithQuestionsCount(_connectionString);
            // Преобразуем к объектам с нужными именами свойств
            var response = result.Select(r => new {
                userId = r.Item1,
                username = r.Item2,
                questionsAdded = r.Item3
            });
            return Ok(response);
        }

        [HttpGet("questions-with-authors")]
        public IActionResult GetQuestionsWithAuthors()
        {
            var result = Queries.getQuestionsWithAuthors(_connectionString);
            var response = result.Select(r => new {
                questionId = r.Item1,
                text = r.Item2,
                addedBy = r.Item3
            });
            return Ok(response);
        }

        [HttpGet("users-with-average-matches")]
        public IActionResult GetUsersWithAverageMatches()
        {
            var rawResult = Queries.getUsersWithAverageMatchesCount(_connectionString);
            var response = rawResult.Select(tuple => new {
                user_id = tuple.Item1,
                username = tuple.Item2,
                avg_matches_count = tuple.Item3
            });
            return Ok(response);
        }


        [HttpPost("next")]
        public IActionResult NextStep([FromBody] StepRequest request)
        {
            if (string.IsNullOrEmpty(request.SessionId))
                request.SessionId = System.Guid.NewGuid().ToString();

            if (!Sessions.ContainsKey(request.SessionId))
                Sessions[request.SessionId] = new SessionData();

            var session = Sessions[request.SessionId];

            if (session.State == GameState.AskingQuestions)
            {
                if (!string.IsNullOrEmpty(request.Answer))
                    session.AnswersGiven.Add(request.Answer);

                string basePath = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
                string args = $"-q -s akinator.pl -g \"next_step({FormatList(session.AnswersGiven)}, Response, Done), write(Response), write('|||'), write(Done), halt.\"";

                string output = RunProlog(args, basePath);
                var parts = output.Split("|||");
                if (parts.Length != 2)
                    return BadRequest("Invalid Prolog response");

                string response = parts[0].Trim();
                bool done = parts[1].Trim() == "true";

                if (response == "no_match")
                {
                    // Начинаем собирать недостающие ответы
                    session.State = GameState.AddingGameQuestions;

                    // Определяем какие вопросы остались
                    int answeredCount = session.AnswersGiven.Count;
                    int remainingCount = 8 - answeredCount;

                    session.AnswersToAdd.Clear();

                    // Возвращаем первый недостающий вопрос
                    if (remainingCount > 0)
                    {
                        string questionText = null;
                        string optionsText = null;

                        // Вызов ask_question для недостающего вопроса
                        int nextQNum = answeredCount + 1;
                        string qArgs = $"-q -s akinator.pl -g \"ask_question({nextQNum}, Q, O), write(Q), write('|||'), write(O), halt.\"";
                        string qOutput = RunProlog(qArgs, basePath);
                        var qParts = qOutput.Split("|||");
                        if (qParts.Length == 2)
                        {
                            questionText = qParts[0];
                            optionsText = qParts[1];
                        }
                        else
                        {
                            questionText = "Error loading question";
                            optionsText = "[]";
                        }

                        return Ok(new StepResponse
                        {
                            SessionId = request.SessionId,
                            Message = questionText + "," + optionsText,
                            // Можно добавить отдельное поле для вариантов, если захочешь
                        });
                    }
                    else
                    {
                        // Неожиданно нет вопросов для добавления
                        return BadRequest("No remaining questions to ask.");
                    }
                }

                if (done)
                {
                    Sessions.TryRemove(request.SessionId, out _);
                }

                return Ok(new StepResponse
                {
                    SessionId = request.SessionId,
                    Message = response,
                });
            }
            else if (session.State == GameState.AddingGameQuestions)
            {
                if (!string.IsNullOrEmpty(request.Answer))
                    session.AnswersToAdd.Add(request.Answer);

                int totalAnswers = session.AnswersGiven.Count + session.AnswersToAdd.Count;

                if (totalAnswers < 8)
                {
                    // Возвращаем следующий вопрос из ask_question
                    string questionText = null;
                    string optionsText = null;

                    int nextQNum = totalAnswers + 1;
                    string basePath = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
                    string qArgs = $"-q -s akinator.pl -g \"ask_question({nextQNum}, Q, O), write(Q), write('|||'), write(O), halt.\"";
                    string qOutput = RunProlog(qArgs, basePath);
                    var qParts = qOutput.Split("|||");
                    if (qParts.Length == 2)
                    {
                        questionText = qParts[0];
                        optionsText = qParts[1];
                    }
                    else
                    {
                        questionText = "Error loading question";
                        optionsText = "[]";
                    }

                    return Ok(new StepResponse
                    {
                        SessionId = request.SessionId,
                        Message = questionText + "," + optionsText,
                    });
                }
                else
                {
                    session.State = GameState.WaitingForName;
                    return Ok(new StepResponse
                    {
                        SessionId = request.SessionId,
                        Message = "Please enter the name of the new game:",
                    });
                }
            }
            else if (session.State == GameState.WaitingForName)
            {
                var gameName = request.Answer?.Trim();
                if (string.IsNullOrEmpty(gameName))
                    return BadRequest("Game name required.");

                var allAnswers = session.AnswersGiven.Concat(session.AnswersToAdd).ToList();

                if (allAnswers.Count != 8)
                    return BadRequest("Incorrect number of answers to add game.");

                string basePath = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
                string formattedAnswers = "[" + string.Join(",", allAnswers.ConvertAll(a => $"'{a}'")) + "]";
                string args = $"-q -s akinator.pl -g \"add_and_save_game('{gameName}', {formattedAnswers}), halt.\"";

                RunProlog(args, basePath);

                Sessions.TryRemove(request.SessionId, out _);

                return Ok(new StepResponse
                {
                    SessionId = request.SessionId,
                    Message = $"Game '{gameName}' added and saved. Thank you!",
                });
            }
            else
            {
                return BadRequest("Unknown session state.");
            }
        }
        [HttpPost("add-distinguishing-game")]
        public IActionResult AddDistinguishingGame([FromBody] AddDistinguishingGameRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.ExistingGameName) ||
                string.IsNullOrWhiteSpace(request.NewGameName) ||
                request.ExistingGameAnswers.Count != request.NewGameAnswers.Count ||
                request.ExistingGameAnswers.Count == 0 ||
                string.IsNullOrWhiteSpace(request.NewQuestionText) ||
                request.NewQuestionOptions.Count == 0)
            {
                return BadRequest("Invalid request.");
            }

            string basePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
            string qText = request.NewQuestionText.Replace("'", "\\'");
            string optionsList = "[" + string.Join(",", request.NewQuestionOptions.Select(o => $"'{o}'")) + "]";
            string addQArgs = $"-q -s akinator.pl -g \"add_question('{qText}', {optionsList}), halt.\"";
            string questionIdOutput = RunProlog(addQArgs, basePath);

            if (!int.TryParse(questionIdOutput, out _))
            {
                return BadRequest($"Failed to add question: Prolog output: {questionIdOutput}");
            }

            var updatedExisting = request.ExistingGameAnswers.Append(request.ExistingGameAnswer).ToList();
            var updatedNew = request.NewGameAnswers.Append(request.NewGameAnswer).ToList();

            string formatList(List<string> list) => "[" + string.Join(",", list.Select(a => $"'{a}'")) + "]";

            string updateGameArgs = $"-q -s akinator.pl -g \"update_game('{request.ExistingGameName}', {formatList(updatedExisting)}), halt.\"";
            RunProlog(updateGameArgs, basePath);

            string addGameArgs = $"-q -s akinator.pl -g \"add_and_save_game('{request.NewGameName}', {formatList(updatedNew)}), halt.\"";
            RunProlog(addGameArgs, basePath);

            return Ok($"Game '{request.NewGameName}' and distinguishing question added successfully.");
        }
        [HttpGet("games")]
        public IActionResult GetAllGames()
        {
            string basePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
            string args = "-q -s akinator.pl -g \"list_games, halt.\"";
            string output = RunProlog(args, basePath);

            var games = output
                .Trim('[', ']')
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(g => g.Trim('\'', ' '))
                .ToList();

            return Ok(games);
        }

        [HttpGet("questions")]
        public IActionResult GetAllQuestions()
        {
            string basePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
            string args = "-q -s akinator.pl -g \"list_questions, halt.\"";
            string output = RunProlog(args, basePath);

            var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var result = new List<QuestionInfo>();

            foreach (var line in lines)
            {
                var parts = line.Split("|||");
                if (parts.Length == 3)
                {
                    if (int.TryParse(parts[0], out int id))
                    {
                        var options = parts[2]
                            .Trim('[', ']')
                            .Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(o => o.Trim('\'', ' '))
                            .ToList();

                        result.Add(new QuestionInfo
                        {
                            Id = id,
                            Text = parts[1].Trim(),
                            Options = options
                        });
                    }
                }
            }

            return Ok(result);
        }


        [HttpPost("count-matches")]
        public async Task<IActionResult> CountMatches([FromBody] CountMatchesRequest request)
        {
            if (request.Answers == null || !request.Answers.Any())
                return BadRequest("Answers required.");

            string basePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
            string formattedAnswers = "[" + string.Join(",", request.Answers.Select(a => $"'{a}'")) + "]";
            string args = $"-q -s akinator.pl -g \"count_possible_games({formattedAnswers}, Count), write(Count), halt.\"";

            string output = RunProlog(args, basePath);

            if (!int.TryParse(output, out int count))
                return BadRequest("Failed to parse Prolog output.");

            var log = new MatchCountLog
            {
                UserId = User.Identity?.IsAuthenticated == true
                         ? int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)
                           ? userId
                           : (int?)null
                         : null,
                RequestedAt = DateTime.UtcNow,
                AnswersJson = JsonSerializer.Serialize(request.Answers ?? new List<string>()),
                MatchesCount = count
            };

            _context.MatchCountLogs.Add(log);
            await _context.SaveChangesAsync();

            return Ok(new CountMatchesResponse { MatchesCount = count });
        }

        private string FormatList(List<string> list)
        {
            if (list.Count == 0) return "[]";
            return "[" + string.Join(",", list.ConvertAll(s => $"'{s}'")) + "]";
        }
    }
}
