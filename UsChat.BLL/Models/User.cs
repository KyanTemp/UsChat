using UsChat.DTO.DTO;

namespace UsChat.BLL.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Password { get; set; }
        public string Email { get; set; }
        public List<Relation>? RelationList { get; set; }

        public User(int Id, string Name, string Password, string Email)
        {
            this.Id = Id;
            this.Name = Name;
            this.Password = Password;
            this.Email = Email;
            this.RelationList = new List<Relation>();
        }
        public User(string Name, string Password, string Email)
        {
            this.Name = Name;
            this.Password = Password;
            this.Email = Email;
        }
        public User(int id, string name, string email)
        {
            Id = id;
            Name = name;
            Email = email;
            this.RelationList = new List<Relation>();
        }
        public User(UserDTO DTO)
        {
            this.Id = DTO.Id;
            this.Name = DTO.Name;
            this.Password = DTO.Password;
            this.Email = DTO.Email;
            this.RelationList = DTO.RelationList?.Select(r => new Relation(new User(r.User), r.Status)).ToList();
        }
        public UserDTO ToDTO()
        {
            UserDTO DTO = new()
            {
                Id = this.Id,
                Name = this.Name,
                Password = this.Password,
                Email = this.Email,
                RelationList = this.RelationList != null  ? this.RelationList.Select(r => new RelationDTO
            {
                User = r.User.ToDTO(),
                Status = r.Status}).ToList()
                : null // Handle the case where RelationList is null
            };
            return DTO;
        }
    }
}
