using System.Data;
using System.Data.SqlClient;
using TravelCoServer.Models;
using TravelCoServer.Repositories.Data;

namespace TravelCoServer.Repositories
{
    public class QuizRepository
    {
        private readonly DbHelper _db;
        public QuizRepository(DbHelper db) { _db = db; }

        public Quiz? GetQuiz(int id)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("TravelCo_sp_Quiz_Get", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);

                SqlDataReader reader = cmd.ExecuteReader();

                // Result set 1: the quiz header
                Quiz? quiz = null;
                if (reader.Read())
                {
                    quiz = new Quiz();
                    quiz.Id = Convert.ToInt32(reader["Id"]);
                    quiz.Title = reader["Title"].ToString();
                    quiz.TimeLimitSec = Convert.ToInt32(reader["TimeLimitSec"]);
                }
                if (quiz == null) return null;   // no quiz with that id

                // Result set 2: the questions
                reader.NextResult();
                Dictionary<int, Question> byId = new Dictionary<int, Question>();
                while (reader.Read())
                {
                    Question q = new Question();
                    q.Id = Convert.ToInt32(reader["Id"]);
                    q.Prompt = reader["Prompt"].ToString();
                    quiz.Questions.Add(q);
                    byId[q.Id] = q;              // remember it so options can find it
                }

                // Result set 3: the options, attached to their question
                reader.NextResult();
                while (reader.Read())
                {
                    int questionId = Convert.ToInt32(reader["QuestionId"]);
                    string optionText = reader["OptionText"].ToString();
                    if (byId.ContainsKey(questionId))
                        byId[questionId].Options.Add(optionText);
                }

                return quiz;
            }
        }

        public Dictionary<int, int> GetAnswers(int quizId)
        {
            Dictionary<int, int> answers = new Dictionary<int, int>();
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("TravelCo_sp_Quiz_GetAnswers", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", quizId);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int qid = Convert.ToInt32(reader["QuestionId"]);
                    int ans = Convert.ToInt32(reader["AnswerIndex"]);
                    answers[qid] = ans;
                }
            }
            return answers;
        }

        public void AddResult(int userId, int quizId, int score, int points)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("TravelCo_sp_QuizResult_Add", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@QuizId", quizId);
                cmd.Parameters.AddWithValue("@Score", score);
                cmd.Parameters.AddWithValue("@Points", points);
                cmd.ExecuteNonQuery();
            }
        }

        public int GetTotalPoints(int userId)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("TravelCo_sp_QuizResults_GetPoints", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", userId);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }
    }
}
