using Microsoft.AspNetCore.Mvc;
using AkinatorApi.Models;
using AkinatorApi.Data;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class QuestionsController : ControllerBase
{
	private readonly AkinatorDbContext _context;

	public QuestionsController(AkinatorDbContext context)
	{
		_context = context;
	}

	[HttpPost]
	public async Task<IActionResult> AddQuestion([FromBody] AddQuestionRequest request)
	{
		if (string.IsNullOrWhiteSpace(request.Text) || request.Options == null || !request.Options.Any())
			return BadRequest("Invalid question data");

		var question = new QuestionInfo
		{
			Text = request.Text,
			Options = request.Options,
			AddedByUserId = request.AddedByUserId,
			AddedAt = DateTime.UtcNow
		};

		_context.Questions.Add(question);
		await _context.SaveChangesAsync();

		return CreatedAtAction(nameof(GetQuestion), new { id = question.Id }, question);
	}

	[HttpGet("{id}")]
	public async Task<IActionResult> GetQuestion(int id)
	{
		var question = await _context.Questions
			.Include(q => q.AddedByUser)
			.FirstOrDefaultAsync(q => q.Id == id);

		if (question == null) return NotFound();

		return Ok(question);
	}
}

public class AddQuestionRequest
{
	public string Text { get; set; } = "";
	public List<string> Options { get; set; } = new();
	public int AddedByUserId { get; set; }
}
