using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using bitOxide.MarioWiiPowerup.Core;
using Bridge.Html5;

namespace bitOxide.MarioWiiPowerup.Javascript
{
    public class ItemImageStore
    {
        private readonly Dictionary<string, ScaledImage> itemImages = new Dictionary<string, ScaledImage>();

        public ItemImageStore()
        {

        }

        public bool LoadCompleted
        {
            get
            {
                foreach (var v in itemImages.Values)
                    if (!v.Complete)
                        return false;

                return true;
            }
        }

        public ScaledImage GetIcon(Item item)
        {
            if (item == null) return null;
            return itemImages.Get(item.Name);
        }

        public ScaledImage GetIcon(string name)
        {
            if (name == null) return null;
            return itemImages.Get(name);
        }

        public void RegisterIcon(Item item, string imageUrl, bool blackBg = false)
        {
            RegisterIcon(item.Name, imageUrl, blackBg);
        }

        public void RegisterIcon(string name, string imageUrl, bool blackBg = false)
        {
            var si = new ScaledImage(new HTMLImageElement() { Src = imageUrl }, blackBg);
            itemImages.Add(name, si);
        }

        public static ItemImageStore GetDefault()
        {
            var iis = new ItemImageStore();

            iis.RegisterIcon(Item.Bowser, "img/bowser.png", true);
            iis.RegisterIcon(Item.MiniBowser, "img/minibowser.png", true);

            iis.RegisterIcon(Item.FireFlower, "img/fireflower.png");
            iis.RegisterIcon(Item.Fly, "img/fly.png");
            iis.RegisterIcon(Item.IceFlower, "img/iceflower.png");
            iis.RegisterIcon(Item.Mini, "img/mini.png");
            iis.RegisterIcon(Item.Mushroom, "img/mushroom.png");
            iis.RegisterIcon(Item.Penguin, "img/pinguin.png");
            iis.RegisterIcon(Item.Star, "img/star.png");

            iis.RegisterIcon("bullet", "img/bullet.png");
            iis.RegisterIcon("question", "img/questionmark.png");
            iis.RegisterIcon("pow", "img/pow.png");

            return iis;
        }
    }
}
