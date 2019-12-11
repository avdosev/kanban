using System;
using SQLite;


namespace DataBase {
    class Kanbans {
        [PrimaryKey, AutoIncrement]
        public Guid id { get; set; }
        public string Title { get; set; }
    }

    class Columns {
        [PrimaryKey, AutoIncrement]
        public Guid id { get; set; }

        public int KanbanId { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }
    }

    class Ticket {
        [PrimaryKey, AutoIncrement]
        public Guid id { get; set; }
        public int ColumnId { get; set; }
        public int ColumnKanbanId { get; set; }

        [MaxLength(60)]
        public string Color { get; set; }
        [MaxLength(300)]
        public string Text { get; set; }

    }
    
}