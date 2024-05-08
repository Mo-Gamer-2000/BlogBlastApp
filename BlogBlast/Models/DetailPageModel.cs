/*
 * This class defines the DetailPageModel record, which represents a detailed page model for a blog post, including related posts.
 */

using BlogBlast.Data.Entities;

namespace BlogBlast.Models
{
    // Represents a detailed page model for a blog post, including related posts.
    public record DetailPageModel(Post post, Post[] RelatedPosts)
    {
        // Creates an empty DetailPageModel.
        // <returns>An empty DetailPageModel.</returns>
        public static DetailPageModel Empty() => new(default, []);

        // Gets a value indicating whether the DetailPageModel is empty.
        public bool IsEmpty => post is null;
    }
}