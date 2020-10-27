using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public MessageRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<PagedList<MessageDto>> GetMesagesForUser(MessageParams messageParams)
        {
            // Create IQuerable
            var query = _context.Messages
                            .OrderByDescending(message => message.MessageSent) // Order By most recent first
                            .AsQueryable();

            // Check container for which type of messages to return
            query = messageParams.Container switch
            {
                "Inbox" => query.Where(u => u.Recipient.UserName == messageParams.Username 
                    && u.RecipientDeleted == false),
                "Outbox" => query.Where(u => u.Sender.UserName == messageParams.Username
                    && u.SenderDeleted == false),
                _ => query.Where(u => u.Recipient.UserName == messageParams.Username
                                    && u.RecipientDeleted ==  false
                                    && u.DateRead == null) // "Have not read the message yet"
            };

            var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);

            return await PagedList<MessageDto>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);

        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages
                .Include(u => u.Sender)
                .Include(u => u.Recipient)
                .SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername)
        {
            // Get messages in message thread
            var messages = await _context.Messages
                .Include(u => u.Sender).ThenInclude(p => p.Photos)
                .Include(u => u.Recipient).ThenInclude(p => p.Photos)
                .Where(m => m.Recipient.UserName == currentUsername // the Recipient is the currentUser
                        && m.RecipientDeleted == false              // and Recipient has not deleted
                        && m.Sender.UserName == recipientUsername   // and "Sender" is who is passed into the method
                        ||                                          // OR
                        m.Recipient.UserName == recipientUsername   // the Sender is the currentUser
                        && m.SenderDeleted == false                 // and Sender has not deleted
                        && m.Sender.UserName == currentUsername)    // and "Recipient" is who is passed into the method
                .OrderBy(m => m.MessageSent)
                .ToListAsync();

            // get any unread messages
            var unreadMessages = messages.Where(m => m.DateRead == null && 
                                                m.RecipientUsername == currentUsername).ToList();

            // Mark any "Unread" messages as "Read"
            if (unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.DateRead = DateTime.Now;
                }

                await _context.SaveChangesAsync();
            }

            return _mapper.Map<IEnumerable<MessageDto>>(messages);

        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0; // greater than '0' returns boolean
        }
    }
}