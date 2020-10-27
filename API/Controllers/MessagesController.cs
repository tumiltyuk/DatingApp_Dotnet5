using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class MessagesController : BaseApiController
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _Mapper;
        public MessagesController(IMessageRepository messageRepository, IUserRepository userRepository, IMapper Mapper)
        {
            _Mapper = Mapper;
            _userRepository = userRepository;
            _messageRepository = messageRepository;
        }

        // Create Message
        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
            // Get Username & Id
            var username = User.GetUsername();

            // check if recipient is name of sender
            if (username == createMessageDto.RecipientUsername.ToLower()) return BadRequest("The cannot send messages to yourself.");

            var sender = await _userRepository.GetUserByUserNameAsync(username);
            var recipient = await _userRepository.GetUserByUserNameAsync(createMessageDto.RecipientUsername);

            // check if recipient exists
            if (recipient == null) return NotFound();

            // Create new message
            var message = new Message
            {
                Sender = sender,
                SenderUsername = sender.UserName,
                Recipient = recipient,
                RecipientUsername = recipient.UserName,
                Content = createMessageDto.Content
            };

            // Send message to DB
            _messageRepository.AddMessage(message);

            // Save changes
            if (await _messageRepository.SaveAllAsync())
                return Ok(_Mapper.Map<MessageDto>(message));

            // If all else fails
            return BadRequest("Failed to send message.");
        }

        // Delete Message
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id) 
        {
            var username = User.GetUsername();
            var message = await _messageRepository.GetMessage(id);

            // if sender or recipient not in message then delete cannot take place
            if (message.Sender.UserName != username
                && message.Recipient.UserName != username) return Unauthorized();

            if (message.Sender.UserName == username) message.SenderDeleted = true;
            if (message.Recipient.UserName == username) message.RecipientDeleted = true;

            // Delete from server
            if (message.SenderDeleted && message.RecipientDeleted) 
                _messageRepository.DeleteMessage(message);

            if (await _messageRepository.SaveAllAsync()) return Ok();

            return BadRequest("Problem deleting the message.");
        }

        // Get Messages for User
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
        {
            messageParams.Username = User.GetUsername();

            // get PagedList of Messages
            var messages = await _messageRepository.GetMesagesForUser(messageParams);

            // Add Pagination Headers
            Response.AddPaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages);

            return messages;
        }

        // Get message Threat (Converstaion between two users)
        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
        {
            var currentUsername = User.GetUsername();
            
            return Ok(await _messageRepository.GetMessageThread(currentUsername, username));

        }

    }
}