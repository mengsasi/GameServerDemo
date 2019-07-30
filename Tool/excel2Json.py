import os
import sys
import codecs
import xlrd #http://pypi.python.org/pypi/xlrd

#python float类型 1 是 1.0
def FloatToString (aFloat):
    if type(aFloat) != float:
        return ""
    strTemp = str(aFloat)
    strList = strTemp.split(".")
    if len(strList) == 1 :
        return strTemp
    else:
        if strList[1] == "0" :
            return strList[0]
        else:
            return strTemp

def IsEmptyLine(paramTable, paramRow, paramFieldCount):
    linecnt = 0
    for i in range(paramFieldCount-1):
        v = paramTable.cell_value(paramRow, i)
        v = str(v)
        linecnt += len(v)
        if linecnt > 0:
            return False

    if linecnt == 0:
        return True
    else:
        return False

def table2json(table, tableName, exportPath):
    nrows = table.nrows
    ncols = table.ncols
    confPath = exportPath + tableName + ".json"
    dir = os.path.dirname(confPath)
    if dir and not os.path.exists(dir):
        os.makedirs(dir)
    f = codecs.open(confPath,"w","utf-8")
    strTmp = ""
    strTmp += "{\n\t\"" + tableName + "\":[\n"

    rs = 0
    f.write(strTmp)
    for r in range(3, nrows):
        if IsEmptyLine(table, r, ncols):  #跳过空行
            continue
        strTmp = "\t\t{ "
        i = 0

        for c in range(ncols):
            propName = table.cell_value(0,c)
            propType = table.cell_value(1,c)
            if propType == "ignore":
                continue
            strCellValue = ""
            CellObj = table.cell_value(r,c)
            
            isString = propType == "string" or propType == "json"
            if type(CellObj) == str:
                strCellValue = CellObj.replace("\\", "\\\\").replace("\"", "\\\"")#
            elif type(CellObj) == float:
                strCellValue = FloatToString(CellObj)
            else:
                strCellValue = str(CellObj)
            
            if i > 0:
                delm = ", "
            else:
                delm = ""

            if isString:
                strTmp += delm + "\""  + propName + "\":\""+ strCellValue + "\""
            else:
                strTmp += delm + "\""  + propName + "\":"+ strCellValue
            i += 1

        strTmp += " }"
        if rs > 0:  #不是第1行
            f.write(",\n")
        f.write(strTmp)
        rs += 1

    strTmp = "\n\t]"
    
    strTmp += "\n}"

    f.write(strTmp)
    f.close()
    print("Create ",tableName," OK")
    return

if __name__ == '__main__':
    if len(sys.argv) < 3:
        print('Usage: %s <excel_file>' % sys.argv[0])
        sys.exit(1)

    print("handle file: %s" % sys.argv[1])
    print("exportPath: %s" % sys.argv[2])
	
    excelFileName = sys.argv[1]
    exportPath = sys.argv[2]
    data = xlrd.open_workbook(excelFileName)
    allSheetNames = data.sheet_names()
    for name in allSheetNames:
        exports = name.split("_")
        if len(exports) > 1:
            if str(exports[1]) == "noexport":
                continue
        table = data.sheet_by_name(name)
        print(name)
        table2json(table, name, exportPath)

    print("All OK")
