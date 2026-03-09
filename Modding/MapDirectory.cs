using Edelweiss.Interop;

namespace Edelweiss.Modding
{
    /// <summary>
    /// A class containing map hierarchy data
    /// </summary>
    public class MapDirectory
    {
        /// <summary>
        /// The name of the map type
        /// </summary>
        public BindableVariable<string> Name = "Default";

        /// <summary>
        /// The directory that the map type is put in
        /// </summary>
        public BindableVariable<string> Directory = "{mapper}/{mod}/";
    }
}