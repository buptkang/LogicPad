   M S B A M L   `   `   `  ����P  LTruthTableUserControl, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;"clr-namespace:LogicPad2.TruthTableLogicPad2.TruthTable  5      '    !LogicPad2.TruthTable.UserControl1   5      W SPresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      8x,http://schemas.microsoft.com/winfx/2006/xaml    5      Hmc;http://schemas.openxmlformats.org/markup-compatibility/2006    5      >d2http://schemas.microsoft.com/expression/blend/2008    5      2local"clr-namespace:LogicPad2.TruthTable    5      -   5          >�Name$  truthTableUserControl��5      ��5
      �� 5   
    �� ImageSource$ Images/metal.jpg��6    j� Stretch$
 None=�64    j� TileMode$
 Tile=�6C    j� Viewport$ 1,1,211,211�6S    j� ViewportUnits$ Absolute=�6j   z�5      V� 5   
   .~�5      �� -   5        UserControlScaleXY�� �� ScaleX$ 1Q�5   9    �� ScaleY$ 1Q�6D   T� 5      -   5        UserControlXY�� T� X$ 0Q�5   8   	 T� Y$	 0Q�6>   5      .��5      �� -   5        
 ��Name$
 controlBorder��5      $��1q�6$   
���   �._�5   
   � -   5         �Name$ Grid0��5       � RowDefinitions 5      �� 5      $
��Auto �6    �� 5      $��28 �6    .+�5      .   (LogicPad2.TruthTable.MSInkAnalysisCanvas  -   5          Name$ truthTableInkCanvas��5   (   $��0��6E    �� 
Background �	#000000006R   �� 5       $��1��6   $��1q�6"   
���   �._�5"      �� -   5        $��controlRegion��5"       �� Orientation$ 
Horizontal=�5#      $��Right=�5$      $	��Top=�6:   .+�5&      �� $��Logic TruthTable��6   $
��Bold"�67   $��Italic$�6I    J� Opacity$	 0.7Q�6\   �� 5(      $��1q�6   
���   �$��0,0,10,0q�6E   ._�5)      �� $ 
Horizontal=�6%   .+�5+      �� -   5         ��Name$ 	TermLabel��5+   $   $��Nmbr Of Terms:��67   �� 5,      -	   5        $��
DeleteTerm��5,   %   .��5-   "   �� $��Images/delete.png��6(   $��15��6C   5.      �� 50      -
   5        $ myTerm��50   $   $��5,0,5,0q�64   $��20��6E   �� 52      -   5        $��AddTerm��52   %   .��53   "   �� $��Images/add.png��6(   $��15��6@   54      56      57      �� 59      -   5         ��Name$ btnSave��59   G   $��0,0,10,0q�6    � ToolTip$ Save As Image��6/   $	 0.5Q�6X   .��5:      �� $��Images/saveAsHS.png��6    5;      �� 5=      -   5        $ 	btnCancel��5=   I   $��0,0,10,0q�6   $ Close InkCanvas��6/   $	 0.5Q�6\   .��5>      �� $��Images/cancel.png��6    5?      �� 5A      -   5        $ btnTransform��5A   G   $��0,0,0,0q�6   $ Move InkCanvas��6.   $	 0.5Q�6]   .��5B      �� $��Images/hand_tool.png��6    5C      �� 5E      -   5        $ btnScale��5E   I   $��10,0,0,0q�6   $ Scale InkCanvas��6/   $	 0.5Q�6[   .��5F      �� $��Images/Scale.png��6    5G      5I      5J      5K      5L      5N      