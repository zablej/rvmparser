using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Lib
{
    public class Colorizer : StoreVisitor
    {

        public Colorizer(Logger logger, string colorAttribute = null)
        {
            this.logger = logger;
            this.colorAttribute = colorAttribute;
        }

        public override void init(Store store)
        {
            colorNameByMaterialId.insert(1, Convert.ToUInt64(store.strings.intern("Black")));
            colorNameByMaterialId.insert(2, Convert.ToUInt64(store.strings.intern("Red")));
            colorNameByMaterialId.insert(3, Convert.ToUInt64(store.strings.intern("Orange")));
            colorNameByMaterialId.insert(4, Convert.ToUInt64(store.strings.intern("Yellow")));
            colorNameByMaterialId.insert(5, Convert.ToUInt64(store.strings.intern("Green")));
            colorNameByMaterialId.insert(6, Convert.ToUInt64(store.strings.intern("Cyan")));
            colorNameByMaterialId.insert(7, Convert.ToUInt64(store.strings.intern("Blue")));
            colorNameByMaterialId.insert(8, Convert.ToUInt64(store.strings.intern("Magenta")));
            colorNameByMaterialId.insert(9, Convert.ToUInt64(store.strings.intern("Brown")));
            colorNameByMaterialId.insert(10, Convert.ToUInt64(store.strings.intern("White")));
            colorNameByMaterialId.insert(11, Convert.ToUInt64(store.strings.intern("Salmon")));
            colorNameByMaterialId.insert(12, Convert.ToUInt64(store.strings.intern("LightGrey")));
            colorNameByMaterialId.insert(13, Convert.ToUInt64(store.strings.intern("Grey")));
            colorNameByMaterialId.insert(14, Convert.ToUInt64(store.strings.intern("Plum")));
            colorNameByMaterialId.insert(15, Convert.ToUInt64(store.strings.intern("WhiteSmoke")));
            colorNameByMaterialId.insert(16, Convert.ToUInt64(store.strings.intern("Maroon")));
            colorNameByMaterialId.insert(17, Convert.ToUInt64(store.strings.intern("SpringGreen")));
            colorNameByMaterialId.insert(18, Convert.ToUInt64(store.strings.intern("Wheat")));
            colorNameByMaterialId.insert(19, Convert.ToUInt64(store.strings.intern("Gold")));
            colorNameByMaterialId.insert(20, Convert.ToUInt64(store.strings.intern("RoyalBlue")));
            colorNameByMaterialId.insert(21, Convert.ToUInt64(store.strings.intern("LightGold")));
            colorNameByMaterialId.insert(22, Convert.ToUInt64(store.strings.intern("DeepPink")));
            colorNameByMaterialId.insert(23, Convert.ToUInt64(store.strings.intern("ForestGreen")));
            colorNameByMaterialId.insert(24, Convert.ToUInt64(store.strings.intern("BrightOrange")));
            colorNameByMaterialId.insert(25, Convert.ToUInt64(store.strings.intern("Ivory")));
            colorNameByMaterialId.insert(26, Convert.ToUInt64(store.strings.intern("Chocolate")));
            colorNameByMaterialId.insert(27, Convert.ToUInt64(store.strings.intern("SteelBlue")));
            colorNameByMaterialId.insert(28, Convert.ToUInt64(store.strings.intern("White")));
            colorNameByMaterialId.insert(29, Convert.ToUInt64(store.strings.intern("Midnight")));
            colorNameByMaterialId.insert(30, Convert.ToUInt64(store.strings.intern("NavyBlue")));
            colorNameByMaterialId.insert(31, Convert.ToUInt64(store.strings.intern("Pink")));
            colorNameByMaterialId.insert(32, Convert.ToUInt64(store.strings.intern("CoralRed")));
            colorNameByMaterialId.insert(33, Convert.ToUInt64(store.strings.intern("Black")));
            colorNameByMaterialId.insert(34, Convert.ToUInt64(store.strings.intern("Red")));
            colorNameByMaterialId.insert(35, Convert.ToUInt64(store.strings.intern("Orange")));
            colorNameByMaterialId.insert(36, Convert.ToUInt64(store.strings.intern("Yellow")));
            colorNameByMaterialId.insert(37, Convert.ToUInt64(store.strings.intern("Green")));
            colorNameByMaterialId.insert(38, Convert.ToUInt64(store.strings.intern("Cyan")));
            colorNameByMaterialId.insert(39, Convert.ToUInt64(store.strings.intern("Blue")));
            colorNameByMaterialId.insert(40, Convert.ToUInt64(store.strings.intern("Magenta")));
            colorNameByMaterialId.insert(41, Convert.ToUInt64(store.strings.intern("Brown")));
            colorNameByMaterialId.insert(42, Convert.ToUInt64(store.strings.intern("White")));
            colorNameByMaterialId.insert(43, Convert.ToUInt64(store.strings.intern("Salmon")));
            colorNameByMaterialId.insert(44, Convert.ToUInt64(store.strings.intern("LightGrey")));
            colorNameByMaterialId.insert(45, Convert.ToUInt64(store.strings.intern("Grey")));
            colorNameByMaterialId.insert(46, Convert.ToUInt64(store.strings.intern("Plum")));
            colorNameByMaterialId.insert(47, Convert.ToUInt64(store.strings.intern("WhiteSmoke")));
            colorNameByMaterialId.insert(48, Convert.ToUInt64(store.strings.intern("Maroon")));
            colorNameByMaterialId.insert(49, Convert.ToUInt64(store.strings.intern("SpringGreen")));
            colorNameByMaterialId.insert(50, Convert.ToUInt64(store.strings.intern("Wheat")));
            colorNameByMaterialId.insert(51, Convert.ToUInt64(store.strings.intern("Gold")));
            colorNameByMaterialId.insert(52, Convert.ToUInt64(store.strings.intern("RoyalBlue")));
            colorNameByMaterialId.insert(53, Convert.ToUInt64(store.strings.intern("LightGold")));
            colorNameByMaterialId.insert(54, Convert.ToUInt64(store.strings.intern("DeepPink")));
            colorNameByMaterialId.insert(55, Convert.ToUInt64(store.strings.intern("ForestGreen")));
            colorNameByMaterialId.insert(56, Convert.ToUInt64(store.strings.intern("BrightOrange")));
            colorNameByMaterialId.insert(57, Convert.ToUInt64(store.strings.intern("Ivory")));
            colorNameByMaterialId.insert(58, Convert.ToUInt64(store.strings.intern("Chocolate")));
            colorNameByMaterialId.insert(59, Convert.ToUInt64(store.strings.intern("SteelBlue")));
            colorNameByMaterialId.insert(60, Convert.ToUInt64(store.strings.intern("White")));
            colorNameByMaterialId.insert(61, Convert.ToUInt64(store.strings.intern("Midnight")));
            colorNameByMaterialId.insert(62, Convert.ToUInt64(store.strings.intern("NavyBlue")));
            colorNameByMaterialId.insert(63, Convert.ToUInt64(store.strings.intern("Pink")));
            colorNameByMaterialId.insert(64, Convert.ToUInt64(store.strings.intern("CoralRed")));
            colorNameByMaterialId.insert(206, Convert.ToUInt64(store.strings.intern("Black")));
            colorNameByMaterialId.insert(207, Convert.ToUInt64(store.strings.intern("White")));
            colorNameByMaterialId.insert(208, Convert.ToUInt64(store.strings.intern("WhiteSmoke")));
            colorNameByMaterialId.insert(209, Convert.ToUInt64(store.strings.intern("Ivory")));
            colorNameByMaterialId.insert(210, Convert.ToUInt64(store.strings.intern("Grey")));
            colorNameByMaterialId.insert(211, Convert.ToUInt64(store.strings.intern("LightGrey")));
            colorNameByMaterialId.insert(212, Convert.ToUInt64(store.strings.intern("DarkGrey")));
            colorNameByMaterialId.insert(213, Convert.ToUInt64(store.strings.intern("DarkSlate")));
            colorNameByMaterialId.insert(214, Convert.ToUInt64(store.strings.intern("Red")));
            colorNameByMaterialId.insert(215, Convert.ToUInt64(store.strings.intern("BrightRed")));
            colorNameByMaterialId.insert(216, Convert.ToUInt64(store.strings.intern("CoralRed")));
            colorNameByMaterialId.insert(217, Convert.ToUInt64(store.strings.intern("Tomato")));
            colorNameByMaterialId.insert(218, Convert.ToUInt64(store.strings.intern("Plum")));
            colorNameByMaterialId.insert(219, Convert.ToUInt64(store.strings.intern("DeepPink")));
            colorNameByMaterialId.insert(220, Convert.ToUInt64(store.strings.intern("Pink")));
            colorNameByMaterialId.insert(221, Convert.ToUInt64(store.strings.intern("Salmon")));
            colorNameByMaterialId.insert(222, Convert.ToUInt64(store.strings.intern("Orange")));
            colorNameByMaterialId.insert(223, Convert.ToUInt64(store.strings.intern("BrightOrange")));
            colorNameByMaterialId.insert(224, Convert.ToUInt64(store.strings.intern("OrangeRed")));
            colorNameByMaterialId.insert(225, Convert.ToUInt64(store.strings.intern("Maroon")));
            colorNameByMaterialId.insert(226, Convert.ToUInt64(store.strings.intern("Yellow")));
            colorNameByMaterialId.insert(227, Convert.ToUInt64(store.strings.intern("Gold")));
            colorNameByMaterialId.insert(228, Convert.ToUInt64(store.strings.intern("LightYellow")));
            colorNameByMaterialId.insert(229, Convert.ToUInt64(store.strings.intern("LightGold")));
            colorNameByMaterialId.insert(230, Convert.ToUInt64(store.strings.intern("YellowGreen")));
            colorNameByMaterialId.insert(231, Convert.ToUInt64(store.strings.intern("SpringGreen")));
            colorNameByMaterialId.insert(232, Convert.ToUInt64(store.strings.intern("Green")));
            colorNameByMaterialId.insert(233, Convert.ToUInt64(store.strings.intern("ForestGreen")));
            colorNameByMaterialId.insert(234, Convert.ToUInt64(store.strings.intern("DarkGreen")));
            colorNameByMaterialId.insert(235, Convert.ToUInt64(store.strings.intern("Cyan")));
            colorNameByMaterialId.insert(236, Convert.ToUInt64(store.strings.intern("Turquoise")));
            colorNameByMaterialId.insert(237, Convert.ToUInt64(store.strings.intern("Aquamarine")));
            colorNameByMaterialId.insert(238, Convert.ToUInt64(store.strings.intern("Blue")));
            colorNameByMaterialId.insert(239, Convert.ToUInt64(store.strings.intern("RoyalBlue")));
            colorNameByMaterialId.insert(240, Convert.ToUInt64(store.strings.intern("NavyBlue")));
            colorNameByMaterialId.insert(241, Convert.ToUInt64(store.strings.intern("PowderBlue")));
            colorNameByMaterialId.insert(242, Convert.ToUInt64(store.strings.intern("Midnight")));
            colorNameByMaterialId.insert(243, Convert.ToUInt64(store.strings.intern("SteelBlue")));
            colorNameByMaterialId.insert(244, Convert.ToUInt64(store.strings.intern("Indigo")));
            colorNameByMaterialId.insert(245, Convert.ToUInt64(store.strings.intern("Mauve")));
            colorNameByMaterialId.insert(246, Convert.ToUInt64(store.strings.intern("Violet")));
            colorNameByMaterialId.insert(247, Convert.ToUInt64(store.strings.intern("Magenta")));
            colorNameByMaterialId.insert(248, Convert.ToUInt64(store.strings.intern("Beige")));
            colorNameByMaterialId.insert(249, Convert.ToUInt64(store.strings.intern("Wheat")));
            colorNameByMaterialId.insert(250, Convert.ToUInt64(store.strings.intern("Tan")));
            colorNameByMaterialId.insert(251, Convert.ToUInt64(store.strings.intern("SandyBrown")));
            colorNameByMaterialId.insert(252, Convert.ToUInt64(store.strings.intern("Brown")));
            colorNameByMaterialId.insert(253, Convert.ToUInt64(store.strings.intern("Khaki")));
            colorNameByMaterialId.insert(254, Convert.ToUInt64(store.strings.intern("Chocolate")));
            colorNameByMaterialId.insert(255, Convert.ToUInt64(store.strings.intern("DarkBrown")));

            colorByName.insert(Convert.ToUInt64(store.strings.intern("Blue")), 0x0000cc);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("blue")), 0x0000cc);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("Pink")), 0xcc919e);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("pink")), 0xcc919e);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("SteelBlue")), 0x4782b5);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("steelblue")), 0x4782b5);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("SandyBrown")), 0xf4a55e);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("sandybrown")), 0xf4a55e);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("Black")), 0x000000);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("black")), 0x000000);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("DarkGrey")), 0x518c8c);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("darkgrey")), 0x518c8c);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("RoyalBlue")), 0x4775ff);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("royalblue")), 0x4775ff);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("White")), 0xffffff);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("white")), 0xffffff);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("Brown")), 0xcc2b2b);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("brown")), 0xcc2b2b);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("Ivory")), 0xedede0);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("ivory")), 0xedede0);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("DarkGreen")), 0x2d4f2d);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("darkgreen")), 0x2d4f2d);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("Salmon")), 0xf97f70);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("salmon")), 0xf97f70);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("BrightOrange")), 0xffa500);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("brightorange")), 0xffa500);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("Chocolate")), 0xed7521);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("chocolate")), 0xed7521);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("BrightRed")), 0xff0000);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("brightred")), 0xff0000);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("Plum")), 0x8c668c);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("plum")), 0x8c668c);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("ForestGreen")), 0x238e23);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("forestgreen")), 0x238e23);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("LightGold")), 0xede8aa);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("lightgold")), 0xede8aa);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("CoralRed")), 0xcc5b44);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("coralred")), 0xcc5b44);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("Indigo")), 0x330066);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("indigo")), 0x330066);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("BlueGrey")), 0x687c93);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("bluegrey")), 0x687c93);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("Gold")), 0xedc933);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("gold")), 0xedc933);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("LightYellow")), 0xededd1);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("lightyellow")), 0xededd1);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("PowderBlue")), 0xafe0e5);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("powderblue")), 0xafe0e5);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("LightGrey")), 0xbfbfbf);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("lightgrey")), 0xbfbfbf);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("Yellow")), 0xcccc00);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("yellow")), 0xcccc00);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("DarkBrown")), 0x8c4414);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("darkbrown")), 0x8c4414);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("DeepPink")), 0xed1189);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("deeppink")), 0xed1189);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("Mauve")), 0x660099);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("mauve")), 0x660099);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("Magenta")), 0xdd00dd);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("magenta")), 0xdd00dd);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("Tomato")), 0xff6347);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("tomato")), 0xff6347);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("Midnight")), 0x2d2d4f);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("midnight")), 0x2d2d4f);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("Orange")), 0xed9900);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("orange")), 0xed9900);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("YellowGreen")), 0x99cc33);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("yellowgreen")), 0x99cc33);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("Aquamarine")), 0x75edc6);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("aquamarine")), 0x75edc6);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("DarkSlate")), 0x2d4f4f);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("darkslate")), 0x2d4f4f);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("Red")), 0xcc0000);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("red")), 0xcc0000);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("Khaki")), 0x9e9e5e);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("khaki")), 0x9e9e5e);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("Wheat")), 0xf4ddb2);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("wheat")), 0xf4ddb2);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("Cyan")), 0x00eded);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("cyan")), 0x00eded);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("Turquoise")), 0x00bfcc);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("turquoise")), 0x00bfcc);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("SpringGreen")), 0x00ff7f);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("springgreen")), 0x00ff7f);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("Grey")), 0xa8a8a8);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("grey")), 0xa8a8a8);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("Green")), 0x00cc00);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("green")), 0x00cc00);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("Beige")), 0xf4f4db);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("beige")), 0xf4f4db);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("OrangeRed")), 0xff7f00);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("orangered")), 0xff7f00);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("Tan")), 0xdb9370);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("tan")), 0xdb9370);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("WhiteSmoke")), 0xf4f4f4);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("whitesmoke")), 0xf4f4f4);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("Maroon")), 0x8e236b);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("maroon")), 0x8e236b);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("NavyBlue")), 0x00007f);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("navyblue")), 0x00007f);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("Violet")), 0xed82ed);
            colorByName.insert(Convert.ToUInt64(store.strings.intern("violet")), 0xed82ed);

            defaultName = store.strings.intern("Default");
            colorByName.insert(Convert.ToUInt64(defaultName), 0x787878);

            if (colorAttribute != null)
            {
                colorAttribute = store.strings.intern(colorAttribute);
            }

            stack = new StackItem[store.groupCountAllocated()];
            stack_p = 0;
        }

        public override void beginGroup(Group group)
        {
            var item = new StackItem();
            if (stack_p == 0)
            {
                item.colorName = defaultName;
                item.color = Convert.ToUInt32(colorByName.get(Convert.ToUInt64(defaultName)));
                item.@override = false;
            }
            else
            {
                item = stack[stack_p - 1];
            }

            if (!item.@override)
            {
                ulong colorName = default;
                if (group.group.material == 0)
                {
                    colorName = Convert.ToUInt64(defaultName);
                }
                else if (colorNameByMaterialId.get(colorName, group.group.material))
                {
                    ulong color = default;
                    if (colorByName.get(color, colorName))
                    {
                        item.colorName = colorName.ToString();
                        item.color = Convert.ToUInt32(color);
                    }
                    else if (!naggedName.get(colorName))
                    {
                        naggedName.insert(colorName, 1);
                        logger(1, "Unrecognized color name %s", (char)colorName);
                    }
                }
                else if (!naggedMaterialId.get(group.group.material))
                {
                    naggedMaterialId.insert(group.group.material, 1);
                    logger(1, "Unrecognized material id %d", group.group.material);
                }
            }

            stack[stack_p++] = item;
        }

        public override void EndGroup()
        {
            Debug.Assert(stack_p != 0);
            stack_p--;
        }

        public override void geometry(Geometry geometry)
        {
            Debug.Assert(stack_p != 0);
            geometry.colorName = stack[stack_p - 1].colorName;
            geometry.color = stack[stack_p - 1].color;
        }

        public override void attribute(string key, string val)
        {
            Debug.Assert(stack_p != 0);
            if (key == colorAttribute)
            {
                ulong color = default;
                if (colorByName.get(color, Convert.ToUInt64(val)))
                {
                    var item = stack[stack_p - 1];
                    item.colorName = val;
                    item.color = Convert.ToUInt32(color);
                    item.@override = true;
                }
                else if (!naggedName.get(Convert.ToUInt64(val)))
                {
                    naggedName.insert(Convert.ToUInt64(val), 1);
                    logger(1, "A Unrecognized color name %s", val);
                }
            }
        }

        private class StackItem
        {
            public string colorName;
            public uint color;
            public bool @override;
        };

        private Arena arena;
        private Map colorNameByMaterialId;
        private Map colorByName;
        private Map naggedMaterialId;
        private Map naggedName;
        private Logger logger;
        private StackItem[] stack = null;
        private uint stack_p = 0;
        private string defaultName = null;
        private string colorAttribute = null;

    }
}
