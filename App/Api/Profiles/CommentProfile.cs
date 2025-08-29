namespace Holocron.App.Api.Profiles;

using AutoMapper;
using Holocron.App.Api.Data.Entities;
using Holocron.App.Api.Models.Requests;

public class CommentProfile : Profile
{
    public CommentProfile()
    {
        CreateMap<NewCommentRequest, CommentEntity>();
    }
}