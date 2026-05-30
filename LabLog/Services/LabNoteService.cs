using Google.Cloud.Firestore;
using LabLog.DTOs;
using LabLog.Models;

namespace LabLog.Services
{
    public class LabNoteService
    {
        private readonly FirebaseService _firebaseService;

        public LabNoteService(FirebaseService firebaseService)
        {
            _firebaseService = firebaseService;
        }

        public async Task<LabNote> Create(LabNoteDto dto, string userId)
        {
            ValidateLabNote(dto);

            var collection = _firebaseService.GetCollection("labnotes");

            var note = new LabNote
            {
                Id = Guid.NewGuid().ToString(),
                Title = dto.Title,
                Observation = dto.Observation,
                Category = dto.Category,
                Priority = dto.Priority,
                IsPublic = dto.IsPublic,
                Tags = dto.Tags,
                CreatedAt = DateTime.UtcNow,
                UserId = userId
            };

            await collection.Document(note.Id).SetAsync(new Dictionary<string, object>
            {
                { "Id", note.Id },
                { "Title", note.Title },
                { "Observation", note.Observation },
                { "Category", note.Category },
                { "Priority", note.Priority },
                { "IsPublic", note.IsPublic },
                { "Tags", note.Tags },
                { "CreatedAt", note.CreatedAt },
                { "UserId", note.UserId }
            });

            return note;
        }

        public async Task<List<LabNote>> GetByUser(string userId)
        {
            var collection = _firebaseService.GetCollection("labnotes");

            var snapshot = await collection
                .WhereEqualTo("UserId", userId)
                .GetSnapshotAsync();

            var notes = new List<LabNote>();

            foreach (var doc in snapshot.Documents)
            {
                var data = doc.ToDictionary();

                notes.Add(new LabNote
                {
                    Id = data["Id"].ToString()!,
                    Title = data["Title"].ToString()!,
                    Observation = data["Observation"].ToString()!,
                    Category = data["Category"].ToString()!,
                    Priority = Convert.ToInt32(data["Priority"]),
                    IsPublic = Convert.ToBoolean(data["IsPublic"]),
                    Tags = data["Tags"].ToString()!,
                    CreatedAt = ((Timestamp)data["CreatedAt"]).ToDateTime(),
                    UserId = data["UserId"].ToString()!
                });
            }

            return notes;
        }

        public async Task<bool> Delete(string id, string userId)
        {
            var collection = _firebaseService.GetCollection("labnotes");

            var document = collection.Document(id);
            var snapshot = await document.GetSnapshotAsync();

            if (!snapshot.Exists)
                throw new Exception("La nota no existe");

            var data = snapshot.ToDictionary();
            var ownerId = data["UserId"].ToString();

            if (ownerId != userId)
                return false;

            await document.DeleteAsync();

            return true;
        }

        private void ValidateLabNote(LabNoteDto dto)
        {
            var validCategories = new List<string>
            {
                "Quimica",
                "Biologia",
                "Fisica",
                "Otro"
            };

            if (!validCategories.Contains(dto.Category))
                throw new Exception("La categoria solo puede ser: Quimica, Biologia, Fisica u Otro");

            if (dto.Priority < 1 || dto.Priority > 3)
                throw new Exception("La prioridad solo puede ser 1, 2 o 3");
        }
    }
}