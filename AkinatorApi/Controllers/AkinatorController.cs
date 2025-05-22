using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using AkinatorApi.Models;

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
                string formattedAnswers = string.Join(",", allAnswers.ConvertAll(a => $"'{a}'"));
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

        private string FormatList(List<string> list)
        {
            if (list.Count == 0) return "[]";
            return "[" + string.Join(",", list.ConvertAll(s => $"'{s}'")) + "]";
        }
    }
}
