   M S B A M L   `   `   `  ����  iMineLauncher*    $iMine.Launcher.Windows.ConsoleWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	     � Title$  iMine Консоль��5   	   $	��600��6   $	��660��6,    � WindowStyle$
 None=�68    � AllowsTransparency . ���	#EE191511 � 
ResizeMode$ CanResizeWithGrip=�6|    � WindowStartupLocation$ CenterOwner=�6�   .��5      � .+�5	   
   �� -   5         ��Name$ 
KillButton��5	   )   $��Остановить��6=   $��Right=�6�   $��Bottom=�6�   $	��100��6�   $��50��6�      BlackButtons#���   6�   $��
0,0,10,-11q�6  �� 5
   
   -   5         ��Name$ 
ConsoleBox��5
   ~    �� 
IsReadOnly . $ �� HorizontalScrollBarVisibility$
 Auto=�6$   "	 �� VerticalScrollBarVisibility$
	 Auto=�6I   
 �� 	MaxLength$
 50000��6l   $��	5,22,5,35q�6�   $�� ��6�   �� 5   
   -   5        
������ $��Stretch=�5   '   $��22��6E   $��0,0,0,0q�6Q   $	��Top=�6b   6z   �� 5   
   -   5        $ CloseButton��5   L     StartMenuButtons#���  6    �� IsCancel . $��Right=�6a   $	��Top=�6}   $��28��6�   $��28��6�   $��0,0,0,0q�6�   6�   5      5      