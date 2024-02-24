﻿using System.Security.Claims;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Repository.IRepository;
using API.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]

public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IPhotoService _photoService;

    public UsersController(IUserRepository userRepository
    , IMapper mapper,
    IPhotoService photoService)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _photoService = photoService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
    {

        var users = await _userRepository.GetMembersAsync();
        return Ok(users);
    }

    [HttpGet("{userName}")]
    public async Task<ActionResult<MemberDto>> GetUser(string userName)
    {
        return await _userRepository.GetMemberAsync(userName);
    }
    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
    {

        var user = await _userRepository.GetUserByUserNameAsync(User.GetUserName());
        if (user == null) return NotFound();
        _mapper.Map(memberUpdateDto, user); // from -- to
        if (await _userRepository.SaveAllAsync()) return NoContent();
        return BadRequest("Failed to update user");
    }
    [HttpPost("add-photo")]
    public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
    {
        var user = await _userRepository.GetUserByUserNameAsync(User.GetUserName());
        if (user == null) return NotFound();

        var result = await _photoService.AddPhotoAsync(file);
        if (result.Error != null) return BadRequest(result.Error.Message);
        var photo = new Photo
        {
            Url = result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId
        };
        if (user.Photos.Count == 0)
            photo.IsMain = true;

        user.Photos.Add(photo);
        if (await _userRepository.SaveAllAsync())
        {
            // return the location of the new resoursce 
            // the get method url
            return CreatedAtAction(nameof(GetUser),
             new { userName = user.UserName },
             _mapper.Map<PhotoDto>(photo));
        }
        return BadRequest("Problem adding photo");
    }
    [HttpPut("set-main-photo/{photoId}")]
    public async Task<ActionResult<PhotoDto>> SetMainPhoto(int photoId)
    {
        var user = await _userRepository.GetUserByUserNameAsync(User.GetUserName());
        if (user == null) return NotFound();
        var photo = user.Photos.FirstOrDefault(u => u.Id == photoId);
        if (photo == null) return NotFound();
        if (photo.IsMain) return BadRequest("Already main photo");

        var currentMain = user.Photos.FirstOrDefault(u => u.IsMain == true);
        if (currentMain != null) currentMain.IsMain = false;

        photo.IsMain = true;
        if (await _userRepository.SaveAllAsync())
            return NoContent();
        return BadRequest("Problem setting main photo");
    }
    [HttpDelete("delete-photo/{photoId}")]
    public async Task<ActionResult> DeletePhoto(int photoId)
    {
        var user = await _userRepository.GetUserByUserNameAsync(User.GetUserName());
        var photo = user.Photos.FirstOrDefault(u => u.Id == photoId);
        if (photo == null) NotFound();
        if (photo.IsMain) return BadRequest("You cannot delete your main photo");
        if (photo.PublicId != null)
        {
            var result = await _photoService.DeletePhotoAsync(photo.PublicId);
            if (result.Error != null)
            {
                return BadRequest(result.Error.Message);
            }
        }
        user.Photos.Remove(photo);
        if (await _userRepository.SaveAllAsync())
            return Ok();
        return BadRequest("Problem deleting photo");
    }



}
