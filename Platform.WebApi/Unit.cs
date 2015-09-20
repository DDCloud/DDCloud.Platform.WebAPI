namespace DDCloud.Platform.WebApi
{
	/// <summary>
	///		Represents <c>void</c>; lack of a value.
	/// </summary>
	public struct Unit
	{
		/// <summary>
		///		Singleton value for <see cref="Unit"/>.
		/// </summary>
		public static readonly Unit Value = new Unit();
	}
}
