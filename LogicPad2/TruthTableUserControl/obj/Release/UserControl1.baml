   M S B A M L   `   `   `  ���� P  LTruthTableUserControl, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;"clr-namespace:LogicPad2.TruthTableLogicPad2.TruthTable  '    !LogicPad2.TruthTable.UserControl1   R NWindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    8x,http://schemas.microsoft.com/winfx/2006/xaml    Hmc;http://schemas.openxmlformats.org/markup-compatibility/2006    >d2http://schemas.microsoft.com/expression/blend/2008    2local"clr-namespace:LogicPad2.TruthTable    -     >�Name$  truthTableUserControl������  �� ImageSource$ Images/metal.jpg�� j� Stretch$
 None=� j� TileMode$
 Tile=� j� Viewport$ 1,1,211,211� j� ViewportUnits$ Absolute=�z�V� .~��� -   UserControlScaleXY�� �� ScaleX$ 1Q� �� ScaleY$ 1Q�T� -   UserControlXY�� T� X$ 0Q�	 T� Y$	 0Q�.���� -   
 ��Name$
 controlBorder��$��1q�
���   �._�� -    �Name$ Grid0�� � RowDefinitions �� $
��Auto ��� $��28 �.+�.   (LogicPad2.TruthTable.MSInkAnalysisCanvas  -     Name$ truthTableInkCanvas��$��0�� �� 
Background �	#00000000�� $��1��$��1q�
���   �._��� -   $��controlRegion�� �� Orientation$ 
Horizontal=�$��Right=�$	��Top=�.+��� $��Logic TruthTable��$
��Bold"�$��Italic$� J� Opacity$	 0.7Q��� $��1q�
���   �$��0,0,10,0q�._��� $ 
Horizontal=�.+��� -    ��Name$ 	TermLabel��$��Nmbr Of Terms:���� -	   $��
DeleteTerm��.���� $��Images/delete.png��$��15���� -
   $ myTerm��$��5,0,5,0q�$��20���� -   $��AddTerm��.���� $��Images/add.png��$��15���� -    ��Name$ btnSave��$��0,0,10,0q� � ToolTip$ Save As Image��$	 0.5Q�.���� $��Images/saveAsHS.png���� -   $ 	btnCancel��$��0,0,10,0q�$ Close InkCanvas��$	 0.5Q�.���� $��Images/cancel.png���� -   $ btnTransform��$��0,0,0,0q�$ Move InkCanvas��$	 0.5Q�.���� $��Images/hand_tool.png���� -   $ btnScale��$��10,0,0,0q�$ Scale InkCanvas��$	 0.5Q�.���� $��Images/Scale.png��