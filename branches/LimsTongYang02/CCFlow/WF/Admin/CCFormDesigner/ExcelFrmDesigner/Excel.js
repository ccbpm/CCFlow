//约定：
//1.单个单元格：Cell
//2.多个单元格：Range
var Excel = function (wo) {
	this.wo = wo;
	this.doExcel = wo.GetDocumentObject();
	this.app = wo.GetDocumentObject().Application;
	this.names = {};
};
Excel.prototype = {

	//是否选中单个单元格（包含合并后的单元格）
	IsSingle: function(){
		this.Log("Excel.IsSingle | Selection.Count: " + this.app.Selection.Count);
		this.Log("Excel.IsSingle | Selection.Address: " + this.GetRange());
		this.Log("Excel.IsSingle | ActiveCell.MergeArea.Address: " + this.app.ActiveCell.MergeArea.Address);
		if(this.app.Selection.Count == 1){
			return true;
		}else{
			return (this.GetRange() == this.app.ActiveCell.MergeArea.Address);
		}
	},
	//获取当前单元格（不带$符号）//e.g. C2
	GetCell_$: function() {
		return this.ConvertInt2Letter(this.app.ActiveCell.Column) + this.app.ActiveCell.Row;
	},
	//获取当前单元格//e.g. $C$2
	GetCell: function() {
		return this.app.ActiveCell.Address;
	},
	//获取当前单元格（带sheet页名）//e.g. Sheet1!$C$2
	GetCellS: function() {
		return this.app.ActiveSheet.Name + "!" + this.GetCell();
	},
	GetCellR1C1: function() {},
	//获取当前单元格（x用于设置命名x update: 命名方法Names.Add()也可传入$A$1格式的参数）//e.g. =Sheet1!R2C3($C$2)
	GetCellR1C1S: function() {
		return "=" + this.GetSheet() + "!R" + this.app.ActiveCell.Row + "C" + this.app.ActiveCell.Column;
	},

	//获取选中区域//e.g. $A$1:$C$2
	GetRange: function() {
		return this.app.Selection.Address;
	},
	//获取选中区域（x用于设置命名x update: 命名方法Names.Add()也可传入$A$1格式的参数）//e.g. =Sheet1!R2C3:R5C6($C$2:$F$5)//若选中单个单元格，则返回值同this.GetCellR1C1S();
	GetRangeR1C1S: function() {
		var range = this.app.Selection;
		var rangeAdd = "=" + this.GetSheet() + "!R" + range.Row + "C" + range.Column;
		if (range.Count > 1) {
			var rEnd = range.Row + range.Rows.Count - 1;
			var cEnd = range.Column + range.Columns.Count - 1;
			rangeAdd += ":R" + rEnd + "C" + cEnd;
		}
		return rangeAdd;
	},

	//获取当前Sheet页名//e.g. Sheet1
	GetSheet: function() {
		return this.app.ActiveSheet.Name;
	},

	//获取单元格的值//sheet:Sheet1//cell:A1/$A$1
	GetValue: function(sheet, cell) {
		if (sheet == null) {
			if (cell == null) {
				return this.app.ActiveCell.Value;
			} else {
				return this.app.Range(cell).Value;
			}
		} else {
			return this.app.Sheets(sheet).Range(cell).Value;
		}
	},
	//获取单元格命名//sheet/cell:$A$1
	GetName: function(sheet, cell) {
		sheet = sheet ? sheet : this.GetSheet();
		cell = cell ? cell : this.GetCell();
		//this.Log("Excel.GetName | sheet: " + sheet);
		//this.Log("Excel.GetName | cell: " + cell);
		this.Log("Excel.GetName | Names.Count: " + this.app.Names.Count);
		for (var i = 1; i <= this.app.Names.Count; i++) {
			this.Log("Excel.GetName | for loop | Names.Item(" + i + ").Name: " + this.app.Names.Item(i).Name);
			this.Log("Excel.GetName | for loop | Names.Item(" + i + ").RefersToLocal: " + this.app.Names.Item(i).RefersToLocal);
			if (this.app.Names.Item(i).RefersToLocal == ("=" + sheet + "!" + cell))
				return this.app.Names.Item(i).Name.replace("=" + sheet + "!", "");
		}
		return null;
	},
	//获取区域命名//sheet/range(e.g. $A$1:$C$2,与命名的区域完全匹配时才能取到)
	GetRangeName: function(sheet, range) {
		sheet = sheet ? sheet : this.GetSheet();
		range = range ? range : this.GetRange();
		this.Log("Excel.GetRangeName | Names.Count: " + this.app.Names.Count);
		for (var i = 1; i <= this.app.Names.Count; i++) {
			this.Log("Excel.GetRangeName | for loop | Names.Item(" + i + ").Name: " + this.app.Names.Item(i).Name);
			this.Log("Excel.GetRangeName | for loop | Names.Item(" + i + ").RefersToLocal: " + this.app.Names.Item(i).RefersToLocal);
			if (this.app.Names.Item(i).RefersToLocal == ("=" + sheet + "!" + range))
				return this.app.Names.Item(i).Name.replace("=" + sheet + "!", "");
		}
		return null;
	},
	//获取单元格所在区域的命名（用于判断单元格是否在某一子表范围内）//sheet/cell:$A$1
	GetCellInName: function (sheet, cell) {
		sheet = sheet ? sheet : this.GetSheet();
		cell = cell ? cell : this.GetCell();
		this.Log("Excel.GetCellInName | Names.Count: " + this.app.Names.Count);
		for (var i = 1; i <= this.app.Names.Count; i++) {
			this.Log("Excel.GetCellInName | for loop | Names.Item(" + i + ").Name: " + this.app.Names.Item(i).Name);
			this.Log("Excel.GetCellInName | for loop | Names.Item(" + i + ").RefersToLocal: " + this.app.Names.Item(i).RefersToLocal);
			var range = this.app.Names.Item(i).RefersToLocal; //e.g. =Sheet1!$B$3:$E$5
			if (range.indexOf(":") < 0) { //range不是“区域”时
				continue;
			}else if(range.indexOf(sheet) < 0) { //range不在『当前单元格所在sheet页』时
				continue;
			} else {
				var aryRange = range.replace(":","").split("$");
				if (aryRange.length < 5) { //排除 range 为『单/多整行/列』（e.g. =MetaData!$A:$B）的情况
					continue;
				} else {
					var rMin = aryRange[2], rMax = aryRange[4], cMin = this.ConvertLetter2Int(aryRange[1]), cMax = this.ConvertLetter2Int(aryRange[3]);
					var aryCell = cell.split("$");
					var r = aryCell[2], c = this.ConvertLetter2Int(aryCell[1]);
					if (r < rMin || r > rMax || c < cMin || c > cMax) {
						continue;
					} else {
						return this.app.Names.Item(i).Name;
					}
				}
			}
		}
		return null;
	},
	//根据命名获取address
	GetRangeByName: function(name){
		// for (var i = 1; i <= this.app.Names.Count; i++) {
		// 	if(this.app.Names.Item(i).Name == name){
		// 		return this.app.Names.Item(name).RefersToLocal; //e.g. =Sheet1!$B$3:$E$5
		// 	}
		// }
		// return null;
		try{
			return this.app.Names.Item(name).RefersToLocal; //e.g. =Sheet1!$B$3:$E$5
		}catch(e){
			return null;
		}
	},

	//设置选中范围//'Sheet1', "C2"/$C$2/$B$3:$E$5
	SetSelection: function(sheet, range) {
		if (!sheet) {
			console.error("Excel.SetSelection | Parameter Error | sheet: " + sheet);
			return "参数[sheet]错误";
		}
		if (!range) {
			console.error("Excel.SetSelection | Parameter Error | range: " + range);
			return "参数[range]错误";
		}
		this.app.Sheets(sheet).Select();
		this.app.Range(range).Select();
		return true;
	},
	//设置单元格的值
	SetValue: function(value, sheet, cell) {
		if (value != null) {
			sheet = sheet ? sheet : this.GetSheet();
			cell = cell ? cell : this.GetCell();
			this.app.Sheets(sheet).Range(cell).Value = value;
			return true;
		} else {
			console.error("Excel.SetValue | Parameter Error | value: " + value);
			return "参数[value]错误";
		}
	},
	//设置单元格命名//name/sheet/cell(e.g. $A$1)
	SetName: function(name, sheet, cell) {
		sheet = sheet ? sheet : this.GetSheet();
		cell = cell ? cell : this.GetCell();
		if (this.GetName(sheet, cell) != null) {
			console.warn("Excel.SetCellName | false: " + "该单元格已存在命名！");
			return "该单元格已存在命名！";
		}
		//var area = this.GetCellR1C1S();
		//this.Log("Excel.SetCellName | area: " + area);
		// 测试传入带“$”符号的cell【能】正常执行
		this.app.Names.Add(name, "=" + sheet + "!" + cell);
		return true;
	},
	//设置区域命名//name/sheet/range(e.g. $A$1:$C$2/$E:$E/$7:$7)
	SetRangeName: function(name, sheet, range) {
		sheet = sheet ? sheet : this.GetSheet();
		range = range ? range : this.GetRange();
		//this.Log("Excel.SetRangeName | range: " + range);
		if (this.GetRangeName(sheet, range) != null) {
			console.warn("Excel.SetRangeName | false: " + "该区域已存在命名！");
			return "该区域已存在命名！";
		}
		//if (range.indexOf("$") > -1) {
		//	range = this.ConvertAdd$toR1C1(range);
		//}
		// 实测传入带“$”符号的range【能】正常执行
		this.app.Names.Add(name, "=" + sheet + "!" + range);
	},
	//删除命名
	DelName: function(name){
		if(name){
			try{
				var a = this.app.Names.Item(name).Delete();
				this.Log("Excel.DelName | " + a);
				return true;
			}catch(e){ //命名不存在时
				return true;
			}
		}
		return false;
	},
	//设置单元格【数据有效性】//只支持“序列”、“=命名”的方式
	SetValidation: function(name, sheet, cell) {
		if (name != null) {
			sheet = sheet ? sheet : this.GetSheet();
			cell = cell ? cell : this.GetCell();
			this.app.Sheets(sheet).Range(cell).Validation.Delete();
			this.app.Sheets(sheet).Range(cell).Validation.Add(3, 1, 1, "=" + name);
			return true;
		} else {
			console.error("Excel.SetValidation | Parameter Error | name: " + name);
			return "参数[name]错误";
		}
	},
	//SetRangeValidation: function() {} //设置区域【数据有效性】
	//数字转换为列坐标//e.g. 1->Z, 26->Z, 27->AA
	ConvertInt2Letter: function (i) {
		//只考虑A~ZZ
		//只传入ActiveCell.Column，故未做值验证
		if (i <= 26) {
			return String.fromCharCode(64 + parseInt(i));
		} else {
			return String.fromCharCode(64 + parseInt(i / 26)) + String.fromCharCode(64 + parseInt(i % 26));
		}
	},
	//数字转换为列坐标//e.g. 1->Z, 26->Z, 27->AA
	ConvertLetter2Int: function (l) {
		//只考虑A~ZZ
		if (l.length > 1) {
			return (l.charCodeAt(0) - 64) * 26 + l.charCodeAt(1) - 64;
		} else {
			return l.charCodeAt(0) - 64;
		}
	},
	ConvertAddR1C1to$: function (add) {
	},
	//将带$格式的单元格/区域地址转换为R1C1格式（x主要用于Names.Add()方法x Add方法支持$格式，暂时无用）
	ConvertAdd$toR1C1: function (add) {
		//e.g.
		//①单元格：	=Sheet1!$A$2		=>	=Sheet1!R2C1
		//②区域：	=Sheet1!$A$2:$E$5	=>	=Sheet1!R2C1:R5C5
		//③多整列:	=Sheet1!$A:$E		=>	=Sheet1!C1:C5
		//④单整列：	=Sheet1!$A:$A		=>	=Sheet1!C1
		//⑤多整行：	=Sheet1!$2:$5		=>	=Sheet1!R2:R5
		//⑥单整行：	=Sheet1!$2:$2		=>	=Sheet1!R2
		if (add.indexOf("$") < 0)
			return add;
		var arrayAdd = add.replace(":", "").split("$");
		var newAdd = arrayAdd[0];
		if (isNaN(arrayAdd[1])) { //匹配：①|②|③|④
			if (isNaN(arrayAdd[2])) { //匹配：③|④
				newAdd += "C" + this.ConvertLetter2Int(arrayAdd[1]); //匹配：④
				if (arrayAdd[1] !== arrayAdd[2]) { //匹配：③
					newAdd += ":C" + this.ConvertLetter2Int(arrayAdd[2]);
				}
			} else { //匹配：①|②
				newAdd += "R" + arrayAdd[2] + "C" + this.ConvertLetter2Int(arrayAdd[1]);
				if (arrayAdd.length > 3) { //匹配：②
					newAdd += ":R" + arrayAdd[4] + "C" + this.ConvertLetter2Int(arrayAdd[3]);
				}
			}
		} else { //匹配：⑤|⑥
			newAdd += "R" + arrayAdd[1]; //匹配：⑥
			if (arrayAdd[1] !== arrayAdd[2]) { //匹配：⑤
				newAdd += ":R" + arrayAdd[2];
			}
		}
		return newAdd;
	},

	//新建sheet页
	AddSheet: function (name) {
		if (this.IsExistsSheet(name) == false) {
			this.Log("Excel.AddSheet | add sheet: " + name);
			this.app.Sheets.Add().Name = name;
			return true;
		}
		this.Log("Excel.AddSheet | sheet exists: " + name);
		return false;
	},
	//隐藏sheet页
	HideSheet: function (sheet) {
		this.app.Sheets.Item(sheet).Visible = false;
	},
	//检查是否存在同名sheet页
	IsExistsSheet: function (sheet) {
		for (var i = 1; i <= this.app.Sheets.Count; i++) {
			this.Log("Excel.IsExistsSheet | for loop | sheets[" + i + "] : " + this.app.Sheets.Item(i).Name);
			if (this.app.Sheets.Item(i).Name == sheet)
				return true;
		}
		return false;
	},
	//实现console.log()
	Log: function (t) {
		if (true) { //是否是debug模式
			console.log(t);
		}
	}
};
var Cell = function (i) {
	this.row = i.row ? i.row : null;
	this.column = i.column ? i.column : null;
	this.add = null; //e.g. C2
	this.add$ = null; //e.g. $C$2
	this.addS$ = null; //e.g. Sheet1!$C$2
	this.name = null; //g.g. BJBH
	this.nameS = null; //g.g. Sheet1!BJBH
};
var Table = function (id) {
	this.FK_MapData = id;
	this.Columns = {};
};
var Form = function (id) {
	this.FK_MapData = id;
	this.Tables = {};
}