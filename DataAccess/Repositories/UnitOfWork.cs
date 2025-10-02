using DataAccess.Data;
using DataAccess.Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public IRoomRepository RoomRepository { get; }
        public IRoomPlayerRepository RoomPlayerRepository { get; }
        public IQuestionRepository QuestionRepository { get; }
        public IAnswerOptionRepository AnswerOptionRepository { get; }
        public IMatchResultRepository MatchResultRepository { get; }
        public IFriendshipRepository FriendshipRepository { get; }
        public IApplicationUserOTPRepository ApplicationUserOTPRepository { get; }

        public UnitOfWork(ApplicationDbContext context, IRoomRepository roomRepository,
            IRoomPlayerRepository roomPlayerRepository, IQuestionRepository questionRepository,
            IAnswerOptionRepository answerOptionRepository,
            IMatchResultRepository matchResultRepository, IFriendshipRepository friendshipRepository, IApplicationUserOTPRepository applicationUserOTPRepository)
        {
            _context = context;
            RoomRepository = roomRepository;
            RoomPlayerRepository = roomPlayerRepository;
            QuestionRepository = questionRepository;
            AnswerOptionRepository = answerOptionRepository;
            MatchResultRepository = matchResultRepository;
            FriendshipRepository = friendshipRepository;
            ApplicationUserOTPRepository = applicationUserOTPRepository;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
