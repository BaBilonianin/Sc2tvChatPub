using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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

        string HeaderDataSkin { get; }

        INotifyPropertyChanged HeaderData { get; }

        FrameworkElement CreateSmile( string id );

        void OnLoad( string ConfigPrefix, ConfigStorage Config );

        void OnConfigApply( string ConfigPrefix, ConfigStorage Config );
    }



}
