using Hjg.Pngcs;
using Hjg.Pngcs.Chunks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace RatChat.Skins {
    public static class PngSkin {
        static PngSkin() {
            PngChunk.FactoryRegister(PngChunkSKIN.ID, typeof(PngChunkSKIN));
        }

        public static ResourceDictionary LoadFromPng( string FileName ) {
          
            // read all file
            PngReader pngr = FileHelper.CreatePngReader(FileName);
            pngr.ReadSkippingAllRows();
            pngr.End();
            // we assume there can be at most one chunk of this type... 
            PngChunk chunk = pngr.GetChunksList().GetById1(PngChunkSKIN.ID); // This would work even if not registered, but then PngChunk would be of type PngChunkUNKNOWN

            if (chunk != null) {
                // the following would fail if we had not register the chunk
                PngChunkSKIN chunkprop = (PngChunkSKIN)chunk;
                ParserContext pc = new ParserContext();
                pc.XamlTypeMapper = XamlTypeMapper.DefaultMapper;
              //  pc.XmlSpace

                //MimeObjectFactory s;

                var rd1 = (ResourceDictionary)XamlReader.Parse(chunkprop.Content);

              // Application.Current.Resources.MergedDictionaries.Add(rd1);

              //  var rd2 = (ResourceDictionary)XamlReader.Parse(chunkprop.Content);

              ////  Application.Current.Resources.MergedDictionaries.Add(rd2);

              //  if (rd1 == rd2) {
              //  }

                return rd1;
            } else {
                return null;
            }
        }

        public static void SaveToPng( string FileName, string ToFileName, string XamlFileName ) {
            PngReader pngr = FileHelper.CreatePngReader(FileName);
            PngWriter pngw = FileHelper.CreatePngWriter(ToFileName, pngr.ImgInfo, true);
            pngw.CopyChunksFirst(pngr, ChunkCopyBehaviour.COPY_ALL_SAFE);
            PngChunkSKIN mychunk = new PngChunkSKIN(pngw.ImgInfo);
            mychunk.Content = File.ReadAllText( XamlFileName );
            mychunk.Priority = true; // if we want it to be written as soon as possible
            pngw.GetChunksList().Queue(mychunk);
            for (int row = 0; row < pngr.ImgInfo.Rows; row++) {
                ImageLine l1 = pngr.ReadRow(row);
                pngw.WriteRow(l1, row);
            }
            pngw.CopyChunksLast(pngr, ChunkCopyBehaviour.COPY_ALL);
            pngr.End();
            pngw.End();
        }


        // Example chunk: this stores a serializable object
        public class PngChunkSKIN : PngChunkSingle {
            // ID must follow the PNG conventions: four ascii letters,
            // ID[0] : lowercase (ancillary)
            // ID[1] : lowercase if private, upppecase if public
            // ID[3] : uppercase if "safe to copy"
            public readonly static String ID = "skIn";

            public string Content { get; set; }

            public PngChunkSKIN( ImageInfo info )
                : base(ID, info) {
            }

            public override ChunkOrderingConstraint GetOrderingConstraint() {
                // change this if you don't require this chunk to be before IDAT, etc
                return ChunkOrderingConstraint.BEFORE_IDAT;
            }

            // in this case, we have that the chunk data corresponds to the serialized object
            public override ChunkRaw CreateRawChunk() {
                ChunkRaw c = null;
                byte[] arr = Encoding.UTF8.GetBytes(Content);
                c = createEmptyChunk(arr.Length, true);
                c.Data = arr;
                return c;
            }

            public override void ParseFromRaw( ChunkRaw c ) {
                Content = Encoding.UTF8.GetString(c.Data);
            }

            public override void CloneDataFromRead( PngChunk other ) {
                PngChunkSKIN otherx = (PngChunkSKIN)other;
                this.Content = otherx.Content;
            }

          

        }
    }
}
