using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestAuthentification.ViewModels.User
{
    public class AllUserStateAndUserInWaiting
    {
        // liste des utilisateurs en attente
        public List<UserInfoViewModel> usersInWaiting = new List<UserInfoViewModel>();

        public List<UserInfoViewModel> allUsers = new List<UserInfoViewModel>();

    }
}
