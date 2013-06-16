using System;
using Mogre;

namespace Strategy.MogreControl {
	/// <summary>
	/// Represents the rectagle to plane select.
	/// </summary>
	public class SelectionRectangle : ManualObject {
		/// <summary>
		/// Initializes SelectionRectangle.
		/// </summary>
		/// <param name="name">The name of the SelectionRectangle</param>
		public SelectionRectangle(String name)
			: base(name) {
			// When using this, ensure Depth Check is Off in the material
			RenderQueueGroup = (byte)RenderQueueGroupID.RENDER_QUEUE_OVERLAY; 
			UseIdentityProjection = true;
			UseIdentityView = true;
			QueryFlags = 0;
		}
		
		/// <summary>
		/// Sets the corners of the SelectionRectangle.  Every parameter should be in the
		/// range [0, 1] representing a percentage of the screen the SelectionRectangle
		/// should take up.
		/// </summary>
		/// <param name="left">The left point.</param>
		/// <param name="top">The top point.</param>
		/// <param name="right">The right point.</param>
		/// <param name="bottom">The bottom point.</param>
		void SetCorners(float left, float top, float right, float bottom) {
			left = left * 2 - 1;
			right = right * 2 - 1;
			top = 1 - top * 2;
			bottom = 1 - bottom * 2;
			Clear();
			Begin("", RenderOperation.OperationTypes.OT_LINE_STRIP);
			Position(left, top, -1);
			Position(right, top, -1);
			Position(right, bottom, -1);
			Position(left, bottom, -1);
			Position(left, top, -1);
			End();
			BoundingBox.SetInfinite();
		}

		/// <summary>
		/// Sets corners of the SelectionRectangle.
		/// </summary>
		/// <param name="topLeft">The first corner (top-left).</param>
		/// <param name="bottomRight">The second corner (bottom-right).</param>
		public void SetCorners(Vector2 topLeft, Vector2 bottomRight) {
			SetCorners(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
		}
	}
}
