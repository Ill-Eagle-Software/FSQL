using System;
using System.Collections.Generic;
using System.Data;
using FSQL.ProgramParts.FSQLStructs;

namespace FSQL.Interfaces {
    public interface IActiveRecord {
        string TableName { get; }
        bool IsAtStart { get; }
        bool IsAtEnd { get; }
        bool MoveFirst();
        bool MovePrev();
        dynamic Current { get; }
        int Index { get; }
        bool GoTo(int idx);
        bool MoveNext();
        bool MoveLast();        
        DataRow CurrentRow { get; }        
        DataTable GetData();

        DataTable Filter(Func<dynamic, bool> condition, params ScopedAttribute[] selectedColumns);
        DataTable Filter(Func<DataRow, bool> condition);
        DataTable Join(IActiveRecord that, Func<dynamic, dynamic, bool> condition, params ScopedAttribute[] selectedColumns );
    }
}