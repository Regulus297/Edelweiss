namespace Edelweiss.Mapping.Drawables
{
    /// <summary>
    /// An empty drawable, draws nothing.
    /// Used as a fallback if "_type" is not set when loading a drawable.
    /// </summary>
    public class EmptyDrawable : Drawable
    {
        /// <inheritdoc/>
        public override void Draw()
        {

        }
    }
}