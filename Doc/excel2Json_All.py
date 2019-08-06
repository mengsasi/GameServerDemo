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

def table2json(table, tableName):
    nrows = table.nrows
    ncols = table.ncols
    
    strTmp = ""
    strTmp += "\t\"" + tableName + "\":[\n"

    rs = 3
    for r in range(3, nrows):
        if IsEmptyLine(table, r, ncols):  #跳过空行
            continue
        strTmp += "\t\t{ "
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
        if rs < nrows - 1:
            strTmp += ",\n"
        rs += 1

    strTmp += "\n\t]"
    return strTmp

'''import os
import os.path
rootdir = “d:\data”                                   # 指明被遍历的文件夹

for parent,dirnames,filenames in os.walk(rootdir):    #三个参数：分别返回1.父目录 2.所有文件夹名字（不含路径） 3.所有文件名字
    for dirname in  dirnames:                       #输出文件夹信息
        print "parent is:" + parent
　　　　print  "dirname is" + dirname

    for filename in filenames:                        #输出文件信息
        print "parent is": + parent
        print "filename is:" + filename
　　print "the full name of the file is:" + os.path.join(parent,filename) #输出文件路径信息'''

if __name__ == '__main__':
    if len(sys.argv) < 2:
        print('Usage: %s <excel_file_path>' % sys.argv[0])
        sys.exit(1)

    print("handle file all: %s" % sys.argv[1])
	
    exportPath = sys.argv[1]
	
    confPath = exportPath + "/allConfig.json"
    dir = os.path.dirname(confPath)
    if dir and not os.path.exists(dir):
        os.makedirs(dir)
    
    f = codecs.open(confPath,"w","utf-8")
    json = ""
    json += "{\n"
    
    tc = 0
    f.write(json)
    
    path = os.path.dirname(os.path.abspath(__file__))
    
    for root, dirs, files in os.walk(path):
        for file in files:
            if os.path.splitext(file)[1] == ".xlsx":
                print("excelName is:" + file)
                data = xlrd.open_workbook(file)
                allSheetNames = data.sheet_names()
                
                for name in allSheetNames:
                    exports = name.split("_");
                    if len(exports) > 1:
                        if str(exports[1]) == "noexport":
                            continue
                    if tc > 0:
                        json = ",\n"
                        f.write(json)
                    table = data.sheet_by_name(name)
                    json = table2json(table, name)#路径
                    f.write(json)
                    tc += 1

    f.write("\n}")

    print("All OK")
