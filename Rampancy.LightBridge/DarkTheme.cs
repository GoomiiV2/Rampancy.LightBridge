namespace Rampancy.LightBridge
{
    public static class DarkTheme
    {
        public static Color Light = Color.White;

        public static void ApplyTheme_ContextMenuStrip_Dark(ContextMenuStrip strip)
        {
            foreach (object item in strip.Items)
            {
                if (item.GetType() == typeof(ToolStripSeparator))
                {
                    continue;
                }

                ((ToolStripMenuItem)item).ForeColor = Color.DarkGray;
                ((ToolStripMenuItem)item).BackColor = Color.FromArgb(255, 30, 30, 30);

                foreach (object dropDownItem in ((ToolStripMenuItem)item).DropDownItems)
                {
                    if (dropDownItem.GetType() == typeof(ToolStripSeparator))
                    {
                        ((ToolStripSeparator)dropDownItem).ForeColor = Color.DarkGray;
                        ((ToolStripSeparator)dropDownItem).BackColor = Color.FromArgb(255, 30, 30, 30);
                    }
                    else
                    {
                        ApplyTheme_MenuItems_Dark((ToolStripDropDownItem)dropDownItem);
                    }
                }
            }
        }

        private static void ApplyTheme_MenuStrip_Dark(MenuStrip strip)
        {
            foreach (ToolStripMenuItem item in strip.Items)
            {
                item.ForeColor = Color.DarkGray;
                item.BackColor = Color.FromArgb(255, 25, 25, 25);

                foreach (object dropDownItem in item.DropDownItems)
                {
                    if (dropDownItem.GetType() == typeof(ToolStripSeparator))
                    {
                        ((ToolStripSeparator)dropDownItem).ForeColor = Color.DarkGray;
                        ((ToolStripSeparator)dropDownItem).BackColor = Color.FromArgb(255, 25, 25, 25);
                    }
                    else
                    {
                        ApplyTheme_MenuItems_Dark((ToolStripDropDownItem)dropDownItem);
                    }
                }
            }
        }

        private static void ApplyTheme_MenuItems_Dark(ToolStripDropDownItem item)
        {
            item.ForeColor = Color.DarkGray;
            item.BackColor = Color.FromArgb(255, 25, 25, 25);

            foreach (ToolStripDropDownItem dropDownItem in item.DropDownItems)
            {
                ApplyTheme_MenuItems_Dark(dropDownItem);
            }
        }

        public static void ApplyTheme_Dark(Control parent)
        {
            foreach (Control item in parent.Controls)
            {
                if (item.GetType() == typeof(TextBox))
                {
                    item.ForeColor = Light;
                    item.BackColor = Color.FromArgb(255, 50, 50, 50);
                }
                else if (item.GetType() == typeof(RichTextBox))
                {
                    item.ForeColor = Light;
                    item.BackColor = Color.FromArgb(255, 50, 50, 50);
                }
                else if (item.GetType() == typeof(TreeView))
                {
                    item.ForeColor = Light;
                    item.BackColor = Color.FromArgb(255, 50, 50, 50);
                }
                else if (item.GetType() == typeof(ListView))
                {
                    item.ForeColor = Light;
                    item.BackColor = Color.FromArgb(255, 50, 50, 50);
                }
                else if (item.GetType() == typeof(ContextMenuStrip))
                {
                    item.ForeColor = Light;
                    item.BackColor = Color.FromArgb(255, 30, 30, 30);

                    ApplyTheme_ContextMenuStrip_Dark((ContextMenuStrip)item);
                }
                else if (item.GetType() == typeof(ToolStripMenuItem))
                {
                    item.ForeColor = Light;
                    item.BackColor = Color.FromArgb(255, 25, 25, 25);
                }
                else if (item.GetType() == typeof(ToolStripDropDownItem))
                {
                    item.ForeColor = Light;
                    item.BackColor = Color.FromArgb(255, 25, 25, 25);
                }
                else if (item.GetType() == typeof(ToolStrip))
                {
                    item.ForeColor = Light;
                    item.BackColor = Color.FromArgb(255, 25, 25, 25);
                }
                else if (item.GetType() == typeof(ToolStripSeparator))
                {
                    item.ForeColor = Light;
                    item.BackColor = Color.FromArgb(255, 25, 25, 25);
                }
                else if (item.GetType() == typeof(MenuStrip))
                {
                    item.ForeColor = Light;
                    item.BackColor = Color.FromArgb(255, 25, 25, 25);

                    ApplyTheme_MenuStrip_Dark((MenuStrip)item);
                }
                else if (item.GetType() == typeof(Label))
                {
                    item.ForeColor = Light;
                    item.BackColor = Color.Transparent;
                }
                else if (item.GetType() == typeof(Button))
                {
                    item.ForeColor = Light;
                    item.BackColor = Color.FromArgb(255, 30, 30, 30);
                    ((Button)item).FlatStyle = FlatStyle.Flat;
                }
                else if (item.GetType() == typeof(Form))
                {
                    item.ForeColor = Light;
                    item.BackColor = Color.FromArgb(255, 30, 30, 30);
                }
                else if (item.GetType() == typeof(GroupBox))
                {
                    item.ForeColor = Light;
                    item.BackColor = Color.Transparent;
                }
                else if (item.GetType() == typeof(ComboBox))
                {
                    item.ForeColor = Light;
                    item.BackColor = Color.FromArgb(255, 50, 50, 50);
                    ((ComboBox)item).FlatStyle = FlatStyle.Flat;
                }

                if (item.Controls.Count > 0)
                {
                    ApplyTheme_Dark(item);
                }
            }
        }
    }

    class DarkRenderer : ToolStripProfessionalRenderer
    {
        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            Rectangle rc = new Rectangle(Point.Empty, e.Item.Size);
            Color c = Color.FromArgb(255, 25, 25, 25);
            using (SolidBrush Brush = new SolidBrush(c))
            {
                e.Graphics.FillRectangle(Brush, rc);
            }

            base.OnRenderSeparator(e);
        }

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            int MouseOver = 60;
            int MouseLeave = 25;

            Rectangle rc = new Rectangle(Point.Empty, e.Item.Size);
            Color c = (e.Item.Selected ? Color.FromArgb(255, MouseOver, MouseOver, MouseOver) : Color.FromArgb(255, MouseLeave, MouseLeave, MouseLeave));

            using (SolidBrush Brush = new SolidBrush(c))
            {
                e.Graphics.FillRectangle(Brush, rc);
            }

            //base.OnRenderMenuItemBackground(e);
        }

        protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
        {
            e.ArrowColor = Color.DodgerBlue;

            base.OnRenderArrow(e);
        }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            Rectangle rc = new Rectangle(Point.Empty, e.AffectedBounds.Size);
            Color c = Color.FromArgb(255, 40, 40, 40);

            using (Pen Brush = new Pen(c, 4))
            {
                // Left Side
                e.Graphics.DrawLine(Brush, e.AffectedBounds.Top, e.AffectedBounds.Top, e.AffectedBounds.Top, e.AffectedBounds.Bottom);
                // Right Side
                e.Graphics.DrawLine(Brush, e.AffectedBounds.Width, e.AffectedBounds.Top, e.AffectedBounds.Width, e.AffectedBounds.Bottom);
                // Top Side
                e.Graphics.DrawLine(Brush, e.AffectedBounds.Top, e.AffectedBounds.Top, e.AffectedBounds.Width, e.AffectedBounds.Top);
                // Bottom Sie
                e.Graphics.DrawLine(Brush, e.AffectedBounds.Top, e.AffectedBounds.Bottom, e.AffectedBounds.Width, e.AffectedBounds.Bottom);
            }

            //base.OnRenderToolStripBorder(e);
        }
    }

    class DarkMenuColorTable : ProfessionalColorTable
    {
        public DarkMenuColorTable()
        {
            base.UseSystemColors = false;
        }

        public override Color MenuItemPressedGradientBegin
        {
            get { return Color.FromArgb(255, 40, 40, 40); ; }
        }

        public override Color MenuItemPressedGradientMiddle
        {
            get { return Color.FromArgb(255, 40, 40, 40); ; }
        }

        public override Color MenuItemPressedGradientEnd
        {
            get { return Color.FromArgb(255, 40, 40, 40); ; }
        }

        public override Color ImageMarginGradientBegin
        {
            get { return Color.FromArgb(255, 40, 40, 40); ; }
        }

        public override Color ImageMarginGradientMiddle
        {
            get { return Color.FromArgb(255, 40, 40, 40); ; }
        }

        public override Color ImageMarginGradientEnd
        {
            get { return Color.FromArgb(255, 40, 40, 40); ; }
        }

        public override Color ToolStripDropDownBackground
        {
            get { return Color.FromArgb(255, 40, 40, 40); ; }
        }

        public override Color MenuBorder
        {
            get { return Color.FromArgb(255, 30, 30, 30); ; }
        }

        public override Color MenuItemBorder
        {
            get { return Color.FromArgb(255, 120, 120, 120); ; }
        }

        public override Color MenuItemSelected
        {
            get { return Color.FromArgb(255, 120, 120, 120); ; }
        }

        public override Color MenuItemSelectedGradientBegin
        {
            get { return Color.FromArgb(255, 120, 120, 120); ; }
        }

        public override Color MenuItemSelectedGradientEnd
        {
            get { return Color.FromArgb(255, 120, 120, 120); ; }
        }

        public override Color MenuStripGradientBegin
        {
            get { return Color.FromArgb(255, 40, 40, 40); ; }
        }

        public override Color MenuStripGradientEnd
        {
            get { return Color.FromArgb(255, 40, 40, 40); ; }
        }
    }
}

