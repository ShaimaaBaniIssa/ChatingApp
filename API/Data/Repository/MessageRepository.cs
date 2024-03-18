using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data.Repository.IRepository;
using API.DTOs;
using API.Entities;
using API.Helpers;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data.Repository
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public MessageRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FindAsync(id);
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = _context.Messages
               .OrderBy(u => u.MessageSent).AsQueryable();

            if (messageParams.Container.Equals("Inbox"))
            {
                query = query.Where(
                    u => u.RecipientUserName == messageParams.UserName
                    && u.RecipientDeleted == false);
            }
            else if (messageParams.Container.Equals("Outbox"))
            {
                query = query.Where(
                    u => u.SenderUserName == messageParams.UserName
                    && u.SenderDeleted == false);

            }
            else
            {
                query = query.Where(u => u.RecipientUserName == messageParams.UserName
                && u.DateRead == null
                && u.RecipientDeleted == false);
            }
            var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);
            return await PagedList<MessageDto>.CreateAsync
            (messages, messageParams.PageNumber, messageParams.PageSize);

        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUserName, string recipientUserName)
        {
            var messages = await _context.Messages
            .Include(u => u.Sender).ThenInclude(u => u.Photos)
            .Include(u => u.Recipient).ThenInclude(u => u.Photos)
            .Where(
                m => (m.SenderUserName == currentUserName &&
                m.RecipientUserName == recipientUserName && m.SenderDeleted == false)
                 ||
                (m.SenderUserName == recipientUserName &&
                m.RecipientUserName == currentUserName && m.RecipientDeleted == false)

            )
            .OrderBy(u => u.MessageSent)
            .ToListAsync();

            var unreadMessages = messages.Where
            (u => u.DateRead == null && u.RecipientUserName == currentUserName)
            .ToList();

            if (unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.DateRead = DateTime.UtcNow;
                }
                await _context.SaveChangesAsync();
            }
            return _mapper.Map<IEnumerable<MessageDto>>(messages);

        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}