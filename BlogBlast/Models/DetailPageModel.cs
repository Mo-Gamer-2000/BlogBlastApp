using BlogBlast.Data.Entities;

namespace BlogBlast.Models;

public record DetailPageModel(Post post, Post[] ReleatedPosts);