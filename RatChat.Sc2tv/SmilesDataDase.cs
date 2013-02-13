using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace RatChat.Sc2tv {
    public class SmilesDataDase {
        public const string UpdateSmilesUri = "http://chat.sc2tv.ru/js/smiles.js";

        Dictionary<string, Uri> SmilesUri = new Dictionary<string, Uri>();
        Dictionary<string, BitmapImage> SmilesBmp = new Dictionary<string, BitmapImage>();

        public SmilesDataDase() {
            // А, да. Потом сделать тут загрузку (обязательно в фоне с хитровыдуманным кешированием
            // грузить вот отсюда - UpdateSmilesUri (спорим придется парсить JS или тупо через Regex)


            SmilesUri[":happy: "] = new Uri("http://chat.sc2tv.ru/img/a.png?1");
            SmilesUri[":aws: "] = new Uri("http://chat.sc2tv.ru/img/awesome.png?1");
            SmilesUri[":nc: "] = new Uri("http://chat.sc2tv.ru/img/nocomments.png?1");
            SmilesUri[":manul: "] = new Uri("http://chat.sc2tv.ru/img/manul.png?1");
            SmilesUri[":crazy: "] = new Uri("http://chat.sc2tv.ru/img/crazy.png?1");
            SmilesUri[":cry: "] = new Uri("http://chat.sc2tv.ru/img/cry.png?1");
            SmilesUri[":glory: "] = new Uri("http://chat.sc2tv.ru/img/glory.png?1");
            SmilesUri[":kawai: "] = new Uri("http://chat.sc2tv.ru/img/kawai.png?1");
            SmilesUri[":mee: "] = new Uri("http://chat.sc2tv.ru/img/mee.png?1");
            SmilesUri[":omg: "] = new Uri("http://chat.sc2tv.ru/img/omg.png?1");
            SmilesUri[":whut: "] = new Uri("http://chat.sc2tv.ru/img/mhu.png?1");
            SmilesUri[":sad: "] = new Uri("http://chat.sc2tv.ru/img/sad.png?1");
            SmilesUri[":spk: "] = new Uri("http://chat.sc2tv.ru/img/slowpoke.png?1");
            SmilesUri[":hmhm: "] = new Uri("http://chat.sc2tv.ru/img/2.png?1");
            SmilesUri[":mad: "] = new Uri("http://chat.sc2tv.ru/img/mad.png?1");
            SmilesUri[":angry: "] = new Uri("http://chat.sc2tv.ru/img/aangry.png?1");
            SmilesUri[":xd: "] = new Uri("http://chat.sc2tv.ru/img/ii.png?1");
            SmilesUri[":huh: "] = new Uri("http://chat.sc2tv.ru/img/huh.png?1");
            SmilesUri[":tears: "] = new Uri("http://chat.sc2tv.ru/img/happycry.png?1");
            SmilesUri[":notch: "] = new Uri("http://chat.sc2tv.ru/img/notch.png?1");
            SmilesUri[":vaga: "] = new Uri("http://chat.sc2tv.ru/img/vaganych.png?1");
            SmilesUri[":ra: "] = new Uri("http://chat.sc2tv.ru/img/ra.png?1");
            SmilesUri[":fp: "] = new Uri("http://chat.sc2tv.ru/img/facepalm.png?1");
            SmilesUri[":neo: "] = new Uri("http://chat.sc2tv.ru/img/smith.png?1");
            SmilesUri[":peka: "] = new Uri("http://chat.sc2tv.ru/img/mini-happy.png?3");
            SmilesUri[":trf: "] = new Uri("http://chat.sc2tv.ru/img/trollface.png?2");
            SmilesUri[":fu: "] = new Uri("http://chat.sc2tv.ru/img/fuuuu.png?3");
            SmilesUri[":why: "] = new Uri("http://chat.sc2tv.ru/img/why.png?1");
            SmilesUri[":yao: "] = new Uri("http://chat.sc2tv.ru/img/yao.png?1");
            SmilesUri[":fyeah: "] = new Uri("http://chat.sc2tv.ru/img/fyeah.png?1");
            SmilesUri[":lucky: "] = new Uri("http://chat.sc2tv.ru/img/lol.png?3");
            SmilesUri[":okay: "] = new Uri("http://chat.sc2tv.ru/img/okay.png?2");
            SmilesUri[":alone: "] = new Uri("http://chat.sc2tv.ru/img/alone.png?2");
            SmilesUri[":joyful: "] = new Uri("http://chat.sc2tv.ru/img/ewbte.png?3");
            SmilesUri[":wtf: "] = new Uri("http://chat.sc2tv.ru/img/wtf.png?1");
            SmilesUri[":danu: "] = new Uri("http://chat.sc2tv.ru/img/daladno.png?1");
            SmilesUri[":gusta: "] = new Uri("http://chat.sc2tv.ru/img/megusta.png?1");
            SmilesUri[":bm: "] = new Uri("http://chat.sc2tv.ru/img/bm.png?4");
            SmilesUri[":lol: "] = new Uri("http://chat.sc2tv.ru/img/loool.png?1");
            SmilesUri[":notbad: "] = new Uri("http://chat.sc2tv.ru/img/notbad.png?1");
            SmilesUri[":rly: "] = new Uri("http://chat.sc2tv.ru/img/really.png?1");
            SmilesUri[":ban: "] = new Uri("http://chat.sc2tv.ru/img/banan.png?1");
            SmilesUri[":cap: "] = new Uri("http://chat.sc2tv.ru/img/cap.png?1");
            SmilesUri[":br: "] = new Uri("http://chat.sc2tv.ru/img/br.png?1");
            SmilesUri[":fpl: "] = new Uri("http://chat.sc2tv.ru/img/leefacepalm.png?1");
            SmilesUri[":ht: "] = new Uri("http://chat.sc2tv.ru/img/heart.png?1");
            SmilesUri[":adolf: "] = new Uri("http://chat.sc2tv.ru/img/adolf.png?2");
            SmilesUri[":bratok: "] = new Uri("http://chat.sc2tv.ru/img/bratok.png?1");
            SmilesUri[":strelok: "] = new Uri("http://chat.sc2tv.ru/img/strelok.png?1");
            SmilesUri[":white-ra: "] = new Uri("http://chat.sc2tv.ru/img/white-ra.png?1");
            SmilesUri[":dimaga: "] = new Uri("http://chat.sc2tv.ru/img/dimaga.png?1");
            SmilesUri[":bruce: "] = new Uri("http://chat.sc2tv.ru/img/bruce.png?1");
            SmilesUri[":jae: "] = new Uri("http://chat.sc2tv.ru/img/jaedong.png?1");
            SmilesUri[":flash: "] = new Uri("http://chat.sc2tv.ru/img/flash1.png?1");
            SmilesUri[":bisu: "] = new Uri("http://chat.sc2tv.ru/img/bisu.png?1");
            SmilesUri[":jangbi: "] = new Uri("http://chat.sc2tv.ru/img/jangbi.png?1");
            SmilesUri[":idra: "] = new Uri("http://chat.sc2tv.ru/img/idra.png?1");
            SmilesUri[":vdv: "] = new Uri("http://chat.sc2tv.ru/img/vitya.png?1");
            SmilesUri[":imba: "] = new Uri("http://chat.sc2tv.ru/img/djigurda.png?1");
            SmilesUri[":chuck: "] = new Uri("http://chat.sc2tv.ru/img/chan.png?1");
            SmilesUri[":tgirl: "] = new Uri("http://chat.sc2tv.ru/img/brucelove.png?1");
            SmilesUri[":top1sng: "] = new Uri("http://chat.sc2tv.ru/img/happy.png?1");
            SmilesUri[":slavik: "] = new Uri("http://chat.sc2tv.ru/img/slavik.png?1");
            SmilesUri[":olsilove: "] = new Uri("http://chat.sc2tv.ru/img/olsilove.png?1");
            SmilesUri[":kas: "] = new Uri("http://chat.sc2tv.ru/img/kas.png?1");
            SmilesUri[":pool: "] = new Uri("http://chat.sc2tv.ru/img/pool.png?1");
            SmilesUri[":ej: "] = new Uri("http://chat.sc2tv.ru/img/ejik.png?1");
            SmilesUri[":mario: "] = new Uri("http://chat.sc2tv.ru/img/mario.png?1");
            SmilesUri[":tort: "] = new Uri("http://chat.sc2tv.ru/img/tort.png?1");
            SmilesUri[":arni: "] = new Uri("http://chat.sc2tv.ru/img/terminator.png?1");
            SmilesUri[":crab: "] = new Uri("http://chat.sc2tv.ru/img/crab.png?1");
            SmilesUri[":hero: "] = new Uri("http://chat.sc2tv.ru/img/heroes3.png?1");
            SmilesUri[":mc: "] = new Uri("http://chat.sc2tv.ru/img/mine.png?1");
            SmilesUri[":osu: "] = new Uri("http://chat.sc2tv.ru/img/osu.png?1");
            SmilesUri[":q3: "] = new Uri("http://chat.sc2tv.ru/img/q3.png?1");
            SmilesUri[":tigra: "] = new Uri("http://chat.sc2tv.ru/img/tigrica.png?1");
            SmilesUri[":volck: "] = new Uri("http://chat.sc2tv.ru/img/voOlchik1.png?1");
            SmilesUri[":hpeka: "] = new Uri("http://chat.sc2tv.ru/img/harupeka.png?1");
            SmilesUri[":slow: "] = new Uri("http://chat.sc2tv.ru/img/spok.png?1");
            SmilesUri[":alex: "] = new Uri("http://chat.sc2tv.ru/img/alfi.png?1");
            SmilesUri[":panda: "] = new Uri("http://chat.sc2tv.ru/img/panda.png?1");
            SmilesUri[":sun: "] = new Uri("http://chat.sc2tv.ru/img/sunl.png?1");
            SmilesUri[":cou: "] = new Uri("http://chat.sc2tv.ru/img/cougar.png?2");
            SmilesUri[":wb: "] = new Uri("http://chat.sc2tv.ru/img/wormban.png?1");
            SmilesUri[":dobro: "] = new Uri("http://chat.sc2tv.ru/img/dobre.png?1");
            SmilesUri[":theweedle: "] = new Uri("http://chat.sc2tv.ru/img/weedle.png?1");
            SmilesUri[":apc: "] = new Uri("http://chat.sc2tv.ru/img/apochai.png?1");
            SmilesUri[":globus: "] = new Uri("http://chat.sc2tv.ru/img/globus.png?1");
            SmilesUri[":cow: "] = new Uri("http://chat.sc2tv.ru/img/cow.png?1");
            SmilesUri[":nook: "] = new Uri("http://chat.sc2tv.ru/img/no-okay.png?1");
            SmilesUri[":noj: "] = new Uri("http://chat.sc2tv.ru/img/knife.png?1");
            SmilesUri[":fpd: "] = new Uri("http://chat.sc2tv.ru/img/fp.png?1");
            SmilesUri[":hg: "] = new Uri("http://chat.sc2tv.ru/img/hg.png?1");
            SmilesUri[":yoko: "] = new Uri("http://chat.sc2tv.ru/img/yoko.png?1");
            SmilesUri[":miku: "] = new Uri("http://chat.sc2tv.ru/img/miku.png?1");
            SmilesUri[":winry: "] = new Uri("http://chat.sc2tv.ru/img/winry.png?1");
            SmilesUri[":asuka: "] = new Uri("http://chat.sc2tv.ru/img/asuka.png?1");
            SmilesUri[":konata: "] = new Uri("http://chat.sc2tv.ru/img/konata.png?1");
            SmilesUri[":reimu: "] = new Uri("http://chat.sc2tv.ru/img/reimu.png?1");
            SmilesUri[":sex: "] = new Uri("http://chat.sc2tv.ru/img/sex.png?1");
            SmilesUri[":mimo: "] = new Uri("http://chat.sc2tv.ru/img/mimo.png?1");
            SmilesUri[":fire: "] = new Uri("http://chat.sc2tv.ru/img/fire.png?1");
            SmilesUri[":mandarin: "] = new Uri("http://chat.sc2tv.ru/img/mandarin.png?1");
        }

        public FrameworkElement GetSmile( string id ) {
            // Больше защиты, если сервер смайлов в дауне?
            // если иноплянетяне сломали половину смайлов?
            // если хакеры сменили в JS коды смайлов?
            // кароч, ты понял.

            // Спасибо, хоть за кеширование.

            Image img = new Image() { Height = 24.0, IsHitTestVisible = false };
            BitmapImage bi;

            if (SmilesBmp.TryGetValue(id, out bi)) {
                img.Source = bi;
            } else {
                bi = new BitmapImage(SmilesUri[id]);
                bi.DownloadCompleted += bi_DownloadCompleted;
                img.Source = bi;

                SmilesBmp[id] = bi;
            }

            return img;
        }

        void bi_DownloadCompleted( object sender, EventArgs e ) {
            /// эй, детка, а если картинка не смогла загрузиться, или ваще капец какой, размеры сменили по ширине?
            /// надо бы заапдатить WrapPanel, в которой он сидит, и переместить все группы в чате
        }

    }
}
