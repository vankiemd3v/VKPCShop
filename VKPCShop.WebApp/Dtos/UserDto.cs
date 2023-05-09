namespace VKPCShop.WebApp.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Nhập số điện thoại")]
        public string PhoneNumber { get; set; }
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Nhập địa chỉ")]
        public string Address { get; set; }
        public string Image { get; set; }
        public int RoleId { get; set; }
        public string? IsBuy { get; set; }
        public string? IsBuild { get; set; }
    }
}
