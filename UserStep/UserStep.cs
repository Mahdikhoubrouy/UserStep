using System;
using System.Collections.Generic;

namespace UserStep
{
    public class Step<Tenum>
    {
        public Step()
        {
            Users = new List<UserStepModel<Tenum>>();
        }

        private List<UserStepModel<Tenum>> Users { get; set; }



        /// <summary>
        /// Add User And SetStep
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="Step"></param>
        public void SetStep(long UserId, Tenum Step)
        {
            if (!Users.Exists(x => x.UserId == UserId))
            {
                Users.Add(new UserStepModel<Tenum>() { UserId = UserId, Step = Step });
            }
        }


        /// <summary>
        /// Update User Step
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="Step"></param>
        /// <exception cref="UserNotFound"></exception>
        public void UpdateStep(long UserId, Tenum Step)
        {
            var findUser = Users.Find(x => x.UserId == UserId);
            if (findUser == null)
            {
                throw new UserNotFound("User with this UserId was not found");
            }
            findUser.Step = Step;
        }


        /// <summary>
        /// If the user is deleted successfully, you will receive true, otherwise you will receive false
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="Step"></param>
        /// <returns>True or False</returns>
        public bool RemoveStep(long UserId, Tenum Step)
        {
            return Users.Remove(new UserStepModel<Tenum>() { UserId = UserId, Step = Step });
        }

        /// <summary>
        /// Get user step with UserId
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        /// <exception cref="UserNotFound"></exception>
        public UserStepModel<Tenum> GetStep(long UserId)
        {
            var findUser = Users.Find(x => x.UserId == UserId);
            if (findUser == null)
            {
                throw new UserNotFound("User with this UserId was not found");
            }
            return findUser;
        }

        /// <summary>
        /// Get all user step
        /// </summary>
        /// <returns>List<UserStep></returns>
        public List<UserStepModel<Tenum>> GetAllUser()
        {
            return Users;
        }

    }


    // Step Model
    public class UserStepModel<Tenum>
    {
        public long UserId { get; set; }
        public Tenum Step { get; set; }

    }
}

