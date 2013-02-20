using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace RatChat.Core {
    public delegate void OnNewMessagesArrivedDelegate( List<ChatMessage> NewMessages );

    public interface IChatSource {
        /// <summary>
        /// Автор, копирайт, и прочая чепуха, например:
        /// Oxlamon (c) 2013
        /// </summary>
        string Copyright { get; } 
        /// <summary>
        /// Описание источника чата, например:
        /// Чат для http://sc2tv.ru
        /// </summary>
        string Description { get; }
        /// <summary>
        /// Ник стримера, для выделения того кому пишут
        /// </summary>
        string StreamerNick { get; }
        /// <summary>
        /// Начать работу чата.
        /// </summary>
        void BeginWork();
        /// <summary>
        /// Закончить работу чата.
        /// </summary>
        void EndWork();
        /// <summary>
        /// Вызывается для подписчиков для новых сообщений
        /// </summary>
        event OnNewMessagesArrivedDelegate OnNewMessagesArrived;
        /// <summary>
        /// Текстовый заголовок, его изменения должны быть гарантировано в потоке UI
        /// </summary>
        string Header { get; }
        /// <summary>
        /// Список дополнительных смайлов
        /// </summary>
        Dictionary<string, string> SmilesUri { get; }
        /// <summary>
        /// При загрузке плагина
        /// </summary>
        /// <param name="Config"></param>
        void OnLoad( ConfigStorage Config );
        /// <summary>
        /// После применения настроек
        /// </summary>
        /// <param name="Config"></param>
        void OnConfigApply( ConfigStorage Config );
        /// <summary>
        /// Создать кастомный вид, если возвращает null, то создается VisualChatCtrl
        /// </summary>
        /// <returns></returns>
        UserControl CreateCustomView();
        /// <summary>
        /// Не исполь
        /// </summary>
        string ConfigPrefix { get; set; }
    }
}
