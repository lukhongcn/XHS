using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace XHS.Model
{
    public class ParamterInfo
    {
        private string _sql;
        public string Sql
        {
            get
            {
                return _sql;
            }
            set
            {
                _sql = value;
            }
        }

        private SqlParameter[] _pars;
        public SqlParameter[] Pars
        {
            get
            {
                return _pars;
            }
            set
            {
                _pars = value;
            }
        }

        private CommandType _type;
        public CommandType Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }


        private ArrayList _alsql;
        public ArrayList AlSQL
        {
            get
            {
                return _alsql;
            }
            set
            {
                _alsql = value;
            }
        }

        private ArrayList _alpar;
        public ArrayList AlPAR
        {
            get
            {
                return _alpar;
            }
            set
            {
                _alpar = value;
            }
        }

        private ArrayList _alcom;
        public ArrayList AlCOM
        {
            get
            {
                return _alcom;
            }
            set
            {
                _alcom = value;
            }
        }
    }
}
