using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Miyagi.UI.Controls;
using Miyagi.Common.Data;

namespace Strategy.GameGUI {
    struct MaterialGUIPair {
        public Label name;
        public Label value;

            public MaterialGUIPair(string nameString, int valueInt, int maxWidth, int position) {
            int newWidth = maxWidth/5;
            Console.WriteLine(maxWidth+"THIS IS MAX AND NOW IS NEW "+ newWidth );
            name = new Label() {
                Size = new Size(newWidth*3, 25),
                Text = nameString,
                Location = new Point(0,position*26),
                TextStyle = {
                    Alignment = Miyagi.Common.Alignment.MiddleLeft,
                    ForegroundColour = Colours.White
                },
                Padding = new Thickness(5, 1, 1, 1)
            };
            value = new Label() {
                Size = new Size(newWidth, 25),
                Text = valueInt.ToString(),
                Location = new Point(newWidth*3, position * 26),
                TextStyle = {
                    Alignment = Miyagi.Common.Alignment.MiddleRight,
                    ForegroundColour = Colours.White
                },
                Padding = new Thickness(5,1,1,1)
            };
        }

        public void update(int i) {
            value.Text = i.ToString();
        }
    }
}
