﻿using TadrousManassa.Models;

namespace TadrousManassa.Repositories.IRepositories
{
    public interface IStudentLectureRepository
    {
        public OperationResult<List<Lecture>> GetLecturesByStudent(string studentId);

        public OperationResult<List<Lecture>> GetCurrentLecturesByStudent(string studentId);

        public List<Student> GetStudentsByLecture(string lectureId);

        public Dictionary<string, Dictionary<string, int>> GetViewsCountForStudents();

        public Dictionary<string, int> GetNoWatchers();

        public OperationResult<object> IncrementViewsCount(string studentId, string lectureId);

        public OperationResult<bool> IsLecturePurchased(string studentId, string lectureId);

        public OperationResult<bool> BuyCode(string studentId, string lectureId, string code);

        public int GetRemainingCodes(string lectureId);

        public void CheckRemainingCodes(string lectureId);

        public OperationResult<bool> MarkCodeAsSold(string lectureId, string code);
    }
}
