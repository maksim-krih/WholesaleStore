namespace WholesaleStore.Models.Dtos
{
    public class EmployeeDto : AddressDto
    {
        public int Id { get; set; }
        public int PositionId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }
}