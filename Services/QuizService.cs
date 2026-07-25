using TravelCoServer.Models;
using TravelCoServer.Repositories;

namespace TravelCoServer.Services
{
    public class QuizService
    {
        private readonly QuizRepository _repo;
        public QuizService(QuizRepository repo) { _repo = repo; }

        public Quiz? GetQuiz(int id) { return _repo.GetQuiz(id); }

        public QuizResult Submit(int userId, int quizId, List<Answer> answers)
        {
            Dictionary<int, int> correct = _repo.GetAnswers(quizId);   // questionId -> right index

            int score = 0;
            foreach (Answer a in answers)
            {
                if (correct.ContainsKey(a.QuestionId) && correct[a.QuestionId] == a.SelectedIndex)
                    score++;
            }
            int points = score * 10;

            _repo.AddResult(userId, quizId, score, points);            // save the result
            return new QuizResult { Score = score, Points = points };
        }

        public int GetTotalPoints(int userId) { return _repo.GetTotalPoints(userId); }
    }
}
