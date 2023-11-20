namespace API.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        IMessageRepository MessageRepository { get; }
        ILikesRepository LikesRepository { get; }

        //a unit of work is like a transaction
        //if we had multiple updates to multiple repositories at once
        //this is one transaction
        //we complete to end a transaction and if the transaction doesn't go through, 
        //we roll back all the updates
        //YES, we already do this with save async. Therefore this is abstraction on an abstraction.
        //Doing this to implement it ourselves.
        Task<bool> Complete();

        //does ef see any changes
        bool HasChanges();
    }
}