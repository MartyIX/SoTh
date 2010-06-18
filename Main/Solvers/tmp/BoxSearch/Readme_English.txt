		BoxSearch 5.0 -- Tool for solving Sokoban
		
	NOTE: this program needs MFC7.1 to run. You will get "Missing ***.dll" prompt if MFC7.1 is not installed. 
	You can get MFC7.1 on my homepage:
	http://notabdc.vip.sina.com/Soft/Mfc71.zip
	http://www.freewebs.com/gamesolver/Soft/Mfc71.zip
	Download it and then extract the files in the zip package to the same folder of boxsearch5.
	
== What's new
== How to call BoxSearch from other sokoban game
== Features
== About solve mode
== Usage of the tool
== Contact Author
== My other software
== Recommend my favorite Sokoban game & Links

** For developers **
== How to use BoxSearch solver plugin
== The design principle of BoxSearch


== What's new
	5.0
	More solvable levels & improved speed.
	Support "Plugin mode", which means that you can run BoxSearch as a solver plugin from other application such as Sokoban++.
	
	4.1.3
	Large levels (60*60) supported. Add "zoom in/out" feature to level displaying.

	4.1.2
	Answer optimization can begin at any step of the whole answer.
	
	4.1
	Better support for answer in LrUd format.
	Add new feature: answer optimization. Note: there is no assurance that the optimized answer is the best one. The original answer takes the responsibility that the answer is well organized globally.
	

== How to call BoxSearch from other sokoban game	
	From version 5.0, BoxSearch support "plugin mode", which meas that you can run BoxSearch as a solver plugin from other application such as Sokoban++. To run BoxSearch as plugin, you need plugin dll (BoxSearchPlugin.dll in this package) and follow the instructions of your sokoban program. Usually you need to copy plugin dll to specified directory and do some settings in the game application. Make sure BoxSearch***.exe is also copied to the same directory of the plugin dll.
	Now these sokoban games support solver plugin:
	Sokoban YASC	http://sourceforge.net/projects/sokobanyasc
	Sokoban++		http://www.joriswit.nl/sokoban

	
== Features
	Very quick, Especially for smaller levels. Almost all levels smaller than 10*10 or having less than 6 boxes are solvable.
	Handles large levels well. No limitation for box numbers. 
	4 solve mode: quick, best push, best push with best move, best move with best push. 
	Answer optimization! 
	Run as "plugin mode" -- called by other sokoban game. 

		
== About solve mode
	The main solve mode is used to solve sokoban game QUICKLY, the answer is NOT best-push or best-move. There are other 3 solve modes in my tool to get best-push or best-move answer, but they are much slower, and need more memory. 

	
== Usage of the tool
	Here are some usages not obviously:	
	
	"Edit"-"Destination of people", specify the position of people when this level is solved. It's helpful when solving part of a large map.
	
	The program try to split the whole map into two parts--"box area" & "goal area", if a "cut point" can be found. You can skip one phase in the option dialog.
	
	In "try play" mode, the method to push a box is: click the position behind the box, then move cursor to the destination position of the box, hold down right mouse, then left click.


== Contact Author
	Author: Ge yong
	Email: 
		notabdc@vip.sina.com
		notabdc@hotmail.com
	Homepage:
		www.puzzle007.net
		http://notabdc.vip.sina.com
		
		
== My other software
	You can get following software on my homepage.
	FreecellTool: Solve any freecell game. You don't need to edit game manually. The program catches it from windows for you!
	AutoSweeper: Do minesweeper automatically!
	MineTris: Play minesweeper with endless increasing grids, get much fun than classic minesweeper!
	
	And more, if you can read Chinese, you can get my game solver programs written in Chinese on my Chinese homepage (There are also links on English homepage). They are:
	AutoTetris: play tetris automatically
	AutoJYTK(author: Jin you): tetris with two players attack each other
	AutoRubiksCube(author: Jin you): solve rubiks cube
	Diamond(author: Mao ji & me): solve Peg Solitaire
	HuaRongDaoTool: solve classic Chinese puzzle "HuaRongDao", also known as Klotski
	SiChuanSheng: solve puzzle "SiChuanSheng", very like "Shanghai", in which you can remove a pair when they can connect by no more than 3 lines
	TwelveTool(author: Mao ji): solve classic puzzle "the 12 blocks"
	Wmf(author: Hu bo): Rubics cube on dodecahedron
		
		
== Recommend my favorite Sokoban game & Links
	erim sever's sokoban information collection (http://www.geocities.com/erimsever/sokoban.htm)
	With well arranged links, you can find everything about sokoban here. 
 
	takaken's sokoban solver (http://www.ic-net.or.jp/home/takaken/so/soko/index.html)
	The best sokoban solver for larger levels. 
	
	Sokoban++ (http://www.joriswit.nl/sokoban)
	Creator of "Solver SDK".  
	
	Sokoban YASC (http://sourceforge.net/projects/sokobanyasc)
	The best sokoban software, has many many good features. Open source.  
	
	SuperSoko (http://www.supersoko.com)
	My favorite sokoban software, with many good features and level sets. 


** For developers **

== How to use BoxSearch solver plugin
	you need both BoxSearch5 executable and plugin dll to call BoxSearch solver, and they should be put in same folder. The plugin dll follows "Solver SDK" made by Joris Wit, author of Sokoban++. Visit http://www.joriswit.nl/sokoban/ for the SDK. 


== The design principle of BoxSearch
	Focus on small levels. BoxSearch focus on technics such as deadlock detecting to hanlde small levels well. Technics for large levles such as order arrangement will be covered in the future.
	Self-adapt. There are many internal parameters in the solver, but the user needn't to know it, and nothing is to be config. (in some situation he need to change memory settings to keep enough meory for other important application.)
	Keep every possibility. Some branch-cut technics improve speed at the cost of losing some possibility to reach the goal. BoxSearch doesn't accept this. BoxSearch will try EVERY possible path if time & memory is allowed. By this, BoxSearch has the ablity to declare a level "proved no solution".