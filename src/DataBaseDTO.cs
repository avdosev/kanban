using System;
using System.Collections.Generic;
using SQLite;
using SQLiteNetExtensions.Attributes;


namespace DataBase {
    class Kanbans {
        [PrimaryKey, AutoIncrement]
        public Guid id { get; set; }
        public string Title { get; set; }

        [OneToMany]
        public List<Columns> columns { get; set; }
    }

    class Columns {
        [PrimaryKey, AutoIncrement]
        public Guid id { get; set; }

        [ForeignKey(typeof(Kanbans))]
        public Guid KanbanId { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        [OneToMany]
        public List<Ticket> tickets { get; set; }
    }

    class Ticket {
        [PrimaryKey, AutoIncrement]
        public Guid id { get; set; }
        [ForeignKey(typeof(Columns))]
        public Guid ColumnId { get; set; }
        [ForeignKey(typeof(Kanbans))]
        public Guid ColumnKanbanId { get; set; }

        [MaxLength(60)]
        public string Color { get; set; }
        [MaxLength(300)]
        public string Text { get; set; }

    }
    
}