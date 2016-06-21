using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVCTestAPP.Models;

namespace MVCTestAPP.Repository
{
    public class UserRepository
    {
        private MVCEntities mVCEntities;

        public UserRepository(MVCEntities mVCEntities)
        {
            // TODO: Complete member initialization
            this.mVCEntities = mVCEntities;
        }

        public class UserRepository : IUserRepository
        {
            private MVCEntities context;

            public UserRepository(MVCEntities context)
            {
                this.context = context;
            }

            public IEnumerable<User> GetUsers()
            {
                return context.Users.ToList();
            }

            public User GetUserByID(int userId)
            {
                return context.Users.Find(userId);
            }

            public void InsertUser(User user)
            {
                context.Users.Add(user);
            }

            public void DeleteUser(int userId)
            {
                User user = context.Users.Find(userId);
                context.Users.Remove(user);
            }

            public void UpdateUser(User user)
            {
                context.Entry(user).State = EntityState.Modified;
            }

            public void Save()
            {
                context.SaveChanges();
            }

            private bool disposed = false;

            protected virtual void Dispose(bool disposing)
            {
                if (!this.disposed)
                {
                    if (disposing)
                    {
                        context.Dispose();
                    }
                }
                this.disposed = true;
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
        }
    }
}