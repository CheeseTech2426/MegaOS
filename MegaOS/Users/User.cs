namespace MegaOS {
    public class User {
        public string Username { get; private set; }
        public string Password { get; set; }
        public string Permissions { get; private set; }

        public User(string username, string password, string permissions) {
            Username = username;
            Password = password;
            Permissions = permissions;
        }

        public bool VerifyPassword(string passwordAttempt) {
            return Password == passwordAttempt;
        }

        public override string ToString() {
            return $"{{Username: {Username}, Permissions: {Permissions}}}";
        }
    }
}
