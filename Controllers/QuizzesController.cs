using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TravelCoServer.Models;
using TravelCoServer.Services;

namespace TravelCoServer.Controllers
{
    [ApiController]
    [Route("api/quizzes")]
    public class QuizzesController : ControllerBase
    {
        private readonly QuizService _service;
        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        public QuizzesController(QuizService service) { _service = service; }


        // GET api/quizzes
        [HttpGet("{id}")]
        public ActionResult<Quiz> GetQuiz(int id)
        {
            Quiz? quiz = _service.GetQuiz(id);
            if (quiz == null) return NotFound();
            return Ok(quiz);
        }

        // POST api/quizzes/{id}/submit
        [HttpPost("{id}/submit")]
        [Authorize]
        public ActionResult<QuizResult> Submit(int id, [FromBody] QuizSubmission submission)
        {
            QuizResult result = _service.Submit(CurrentUserId, id, submission.Answers);
            return Ok(result);
        }

        // GET api/quizzes/points
        [HttpGet("points")]
        [Authorize]
        public ActionResult<int> GetPoints()
        {
            return Ok(_service.GetTotalPoints(CurrentUserId));
        }
    }
}
