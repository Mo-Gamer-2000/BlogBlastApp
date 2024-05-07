using BlogBlast.Data.Entities;

namespace BlogBlast.Models
{
    /// <summary>
    /// Represents a detailed page model for a blog post, including related posts.
    /// </summary>
    public record DetailPageModel(Post post, Post[] RelatedPosts)
    {
        /// <summary>
        /// Creates an empty DetailPageModel.
        /// </summary>
        /// <returns>An empty DetailPageModel.</returns>
        public static DetailPageModel Empty() => new(default, []);

        /// <summary>
        /// Gets a value indicating whether the DetailPageModel is empty.
        /// </summary>
        public bool IsEmpty => post is null;
    }
}
