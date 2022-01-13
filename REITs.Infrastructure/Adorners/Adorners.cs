using System.ComponentModel;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace REITs.Infrastructure.Adorners
{
    public class ListViewSortAdorner : Adorner
    {
        public ListSortDirection Direction { get; private set; }

        private readonly Geometry ascendingGeometry =
            Geometry.Parse("M 0,0 L 10,0 L 5,5 Z");

        private readonly Geometry descendingGeometry =
            Geometry.Parse("M 0,5 L 10,5 L 5,0 Z");

        public ListViewSortAdorner(UIElement element, ListSortDirection direction) : base(element)
        {
            this.Direction = direction;
        }

        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (AdornedElement.RenderSize.Width < 20)
            {
                return;
            }

            drawingContext.PushTransform(
                new TranslateTransform(AdornedElement.RenderSize.Width - 15,
                    (AdornedElement.RenderSize.Height - 5) / 2));

            drawingContext.DrawGeometry(
                Brushes.CadetBlue,
                null,
                Direction == ListSortDirection.Ascending ? this.ascendingGeometry : this.descendingGeometry);

            drawingContext.Pop();
        }
    }
}