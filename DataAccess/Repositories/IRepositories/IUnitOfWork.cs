using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.IRepositories
{
    public interface IUnitOfWork : IDisposable
    {
        IRoomRepository RoomRepository { get; }
        IRoomPlayerRepository RoomPlayerRepository { get; }
        IQuestionRepository QuestionRepository { get; }
        IAnswerOptionRepository AnswerOptionRepository { get; }
        IMatchResultRepository MatchResultRepository { get; }
        IFriendshipRepository FriendshipRepository { get; }
    }
}
