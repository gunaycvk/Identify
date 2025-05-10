using IdentityApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace IdentityApp.TagHelpers
{


    [HtmlTargetElement("td", Attributes = "asp-role-users")]


    public class RoleUsersTagHelper : TagHelper
    {

        private readonly RoleManager<AppRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;

        public RoleUsersTagHelper(RoleManager<AppRole> roleManager, UserManager<AppUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }
        [HtmlAttributeName("asp-role-users")]

        public string roleId { get; set; } = null!;

        public string Role { get; set; } = null!;


        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var userNames = new List<string>();
            var role = await _roleManager.FindByIdAsync(roleId);

            if (role != null && role.Name != null)
            {
                var users = _userManager.Users.ToList(); // kritik

                foreach (var user in users)
                {
                    if (await _userManager.IsInRoleAsync(user, role.Name))
                    {
                        userNames.Add(user.UserName ?? "");
                    }
                }

                output.Content.SetContent(userNames.Count == 0 ? "kullanıcı yok" : string.Join(", ", userNames));
            }
            else
            {
                output.Content.SetContent("rol bulunamadı");
            }
        }





    }
}