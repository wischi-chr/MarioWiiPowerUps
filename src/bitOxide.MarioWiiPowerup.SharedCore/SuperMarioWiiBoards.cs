using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace bitOxide.MarioWiiPowerup.Core
{
    public static class SuperMarioWiiBoards
    {
        private static readonly GlobalBoardCollection global = LoadGlobalBoardCollection();
        public static GlobalBoardCollection DefaultBoardSet => global;

#if NET40
        private static char[] GetRawBoardInfosFromEmbeddedResource()
        {
            var asm = Assembly.GetExecutingAssembly();
            var name = asm.GetManifestResourceNames().Where(s => s.EndsWith("MarioBoardData.txt")).First();
            using (var sr = new StreamReader(asm.GetManifestResourceStream(name)))
            {
                var content = string.Join("", sr.ReadToEnd().Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Where(s => !s.StartsWith("//") && !s.StartsWith("[")));
                return content.Where(c => !char.IsWhiteSpace(c) && !char.IsControl(c)).ToArray();
            }
        }
#endif

        private static char[] LoadRawBoardInfos()
        {
#if BRIDGE
            //Inline raw data, because bridge doesn't support embedded resources (I think)
            return "IfFMBbMIfBIfFbIfFFbIFFffFBfMIbIfMBFIbfFMSfMIFBISFbIBFIffIFBfFBFMIbIbMfFIIIFFfbMBFBFSISMfbIfIIMBfFIfMFbFfIBbFmffmSfSFBImbmBFbIffmfmbfBFfImbIFFmBFbfFFffmIfImmmBFbBFbmFFfBmIBIbmIffmIfmfBISfBIfmbbmSFmfFfmBIbBmFfImmIffbIFbBBmSpFppIbmIpffSFImBpmpFmmFpBpbFFbISBpfbpmmpFbBIpfFSIImppmIIBmFbBppFmbIbBBmmpFpmFpmIbFFpIImBmbIImpIpBpbFmpFBfFfbpfbImfIBmFSpSbppfbpmFmmIIBBfFmppfFFmpmbmBIbpmfIpBBpFfmpmbIBfbpmIFmpbfpfSffFmBIIpBFbmSpppFbfmFmmfIpBIbmBIpFISIfBbSBbIpfFmmIfFmBIIBbpFfFpbImFImFmpfIBIpFbFBbfIFBmFIpfffIbBbIpfFImBmbIpfISISFbIBmfpFBfbmBfIfIbFfIpmIpFBSIfBFFSFbbFmIppfmBbmfFbmFFIFpmIpBfmBbIpFfFBIIbFmFpIfmfFmfpFmSISbpIFBBFbfFmpBfmFIIbFIIBpFbfmFfpbmBIbFpmFBIFmfBbIIffSmpBSFbfmFpfFbImmBbIpBmpffmFfBFbfIfBSfpIbFfpmSmfBffmmBmImIbFfpFbpBFfIImfmmmBbpfpFbfBBfImffbmpImFbfmFpmpBbmIfFSSBbmIfFmpFBfbIpIbBpmFmIfIFFFpfBmImFIpBbIfbIFFFBfBIpIbIpmbmIfFFFmpBSmpmbBSmbIffIFFmBfbIImFIpBFIfbFFpbSfFSSBfImSFbIpBmppSbFmfSBbIBmFIpSfSpIfSBSBfImFFbbSSmppIbSmfSfbIBFFSpBmSbIpFBfSBImSmFbSSfppIfFSfSBbISmFbpBmS".ToArray();
#elif NET40
            return GetRawBoardInfosFromEmbeddedResource();
#else
            throw new NotImplementedException();
#endif
        }

        private static GlobalBoardCollection LoadGlobalBoardCollection()
        {
            const int totalNumberOfItems =
                SuperMarioWiiConstants.WorldCount *
                SuperMarioWiiConstants.BoardsPerWorld *
                SuperMarioWiiConstants.ItemsPerBoard;

            int itemCnt = 0;
            var itemRawSource = LoadRawBoardInfos();

            if (itemRawSource.Length != totalNumberOfItems)
                throw new NotSupportedException();

            var worlds = new WorldBoardCollection[SuperMarioWiiConstants.WorldCount];
            for (int world = 0; world < SuperMarioWiiConstants.WorldCount; world++)
            {
                var boards = new Board[SuperMarioWiiConstants.BoardsPerWorld];
                for (int board = 0; board < SuperMarioWiiConstants.BoardsPerWorld; board++)
                {
                    var items = new Item[SuperMarioWiiConstants.ItemsPerBoard];
                    for (int item = 0; item < SuperMarioWiiConstants.ItemsPerBoard; item++)
                        items[item] = GetItemFromChar(itemRawSource[itemCnt++]);
                    boards[board] = new Board(items);
                }
                worlds[world] = new WorldBoardCollection(boards);
            }

            return new GlobalBoardCollection(worlds);
        }

        private static Item GetItemFromChar(char c)
        {
            switch (c)
            {
                case 'I': return Item.IceFlower;
                case 'F': return Item.FireFlower;
                case 'S': return Item.Star;
                case 'm': return Item.Mini;
                case 'B': return Item.Bowser;
                case 'b': return Item.MiniBowser;
                case 'f': return Item.Fly;
                case 'p': return Item.Penguin;
                case 'M': return Item.Mushroom;
            }

            throw new NotSupportedException();
        }
    }
}
