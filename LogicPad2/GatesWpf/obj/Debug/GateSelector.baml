   M S B A M L   `   `   `  ����J  FGatesWpf, Version=1.3.4526.23698, Culture=neutral, PublicKeyToken=null3clr-namespace:GatesWpf.UIGatesGatesWpf.UIGates  5      5clr-namespace:GatesWpf.DragDropGatesWpf.DragDrop  5      O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem 5      #clr-namespace:GatesWpfGatesWpf  5          GatesWpf.GateSelector   5      \ XPresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      8x,http://schemas.microsoft.com/winfx/2006/xaml    5      .gatesclr-namespace:GatesWpf.UIGates    5      2dragDropclr-namespace:GatesWpf.DragDrop    5      4sys&clr-namespace:System;assembly=mscorlib    5      &localclr-namespace:GatesWpf    5        � 	Resources  5	      &    GatesWpf.ExpanderHeightConverter   expanderHeightConverter+   %GatesWpf.DragDrop.GateDragDropAdorner  gateAdorner �� 
TargetType �� Property �� Value `� 	Converter �� Path �� RelativeSource � MappingMode �� 
StartPoint	 �� EndPoint
 � Color � Offset �� Triggers P� Property P� Value N� EnterActions �� TargetProperty W� From	 W� To g� Duration N� ExitActions��%�  &	        &	      '9� &     7     5
   
   6(     5   
   6'   �� 5   
   " 9�6   .�5      ��  � �� 5      `� 5      8   .1�5      �� $ Content.ActualHeight�6"    6>   �� *Self+�� 5      $	 Tag�6"    6-   �� *Self+5      5      �� 5       �@�Tag 5      X�5      1Q�6$   5      �� 5       � �� 5      �� 5      $ Absolute=�6*   $
 55,0&�6A   $	 55,22&�6S   .��5      � $
 White��6'   $	 0.0Q�65   � 5      $
 DarkGray��6'   $	 0.9Q�68   � 5       $
 White��6'   $	 1.0Q�65   5!      5#       5$      P� 5%       �@9�
IsExpanded .  5&      �� 5'      .��5(      �� .��5)   "   W� $	 Tag�5*   2   $ 0X�5+   2   $ 1X�5,   2   $ 0:0:0.25D�5-   2   5.      5/       52      �� 53      .��54      �� .��55   "   W� $	 Tag�56   2   $ 1X�57   2   $ 0X�58   2   $ 0:0:0.25D�59   2   5:      5;      5=      5?      5@      .��5A      �� -   5        $��svGates��5A       $��1��6   .��5B   
   ��  �� Orientation$ Vertical=�6   .+�5C      9� $��Basic Gates��6    9� 
IsExpanded . .��5D      �� $ Vertical=�6   .+�5E         GatesWpf.UIGates.Not  -   5          Name$ tbNot��5E       $	��Top=�6/   &    GatesWpf.DragDrop.DragDropHelper   IsDragSource .    DragDropControl# [ 5G         
DropTarget$ 
gateCanvas��5H         AdornerLayer$ adornerLayer��5I         GatesWpf.UIGates.And  5K      -   5          Name$ tbAnd��5K       $	��Top=�6/    . # [ 5M      $ 
gateCanvas��5N      $ adornerLayer��5O         GatesWpf.UIGates.Or  5R      -   5          Name$
 tbOr��5R      $	��Top=�6-    . # [ 5T      $ 
gateCanvas��5U      $ adornerLayer��5V         GatesWpf.UIGates.Buffer  5X      -   5          Name$ tbBuffer��5X   #   $	��Top=�65    . # [ 5Z      $ 
gateCanvas��5[      $ adornerLayer��5\      5]      5^      9� 5_      $��Compound Gates��6    .  .��5`      �� $ Vertical=�6!   .+�5a         GatesWpf.UIGates.Nand  -   5          Name$ tbNand��5a   !   $	��Top=�61    . # [ 5c      $ 
gateCanvas��5d      $ adornerLayer��5e      	   GatesWpf.UIGates.Nor	  5g      -   5          	 Name$  tbNor��5g       $	��Top=�6/    . # [ 5i      $ 
gateCanvas��5j      $ adornerLayer��5k      
   GatesWpf.UIGates.Xor
  5m      -   5        ! 
 Name$! tbXor��5m       $	��Top=�6/    . # [ 5o      $ 
gateCanvas��5p      $ adornerLayer��5q         GatesWpf.UIGates.Xnor  5s      -	   5        "  Name$" tbXnor��5s   !   $	��Top=�61    . # [ 5u      $ 
gateCanvas��5v      $ adornerLayer��5w      5x      5y      9� 5z      $��	I/O Gates��6    . .��5{      �� $ Vertical=�6   .+�5|         GatesWpf.UIGates.UserInput  -
   5        #  Name$# tbUserInput��5|   &   $	��Top=�6;    . # [ 5~      $ 
gateCanvas��5      $ adornerLayer��5�      !  GatesWpf.UIGates.UserOutput  5�      -   5        $  Name$$ tbUserOutput��5�   '   $	��Top=�6=    . # [ 5�      $ 
gateCanvas��5�      $ adornerLayer��5�      +  %GatesWpf.UIGates.DefaultNumericOutput  5�      -   5        %  Name$% tbNumericOutput��5�   1   $	��Top=�6J    . # [ 5�      $ 
gateCanvas��5�      $ adornerLayer��5�      *   $GatesWpf.UIGates.DefaultNumericInput  5�      -   5        &  Name$& tbNumericInput��5�   0   $	��Top=�6H    . # [ 5�      $ 
gateCanvas��5�      $ adornerLayer��5�         GatesWpf.UIGates.Clock  5�      -   5        '  Name$' tbClock��5�   "   $	��Top=�63    . # [ 5�      $ 
gateCanvas��5�      $ adornerLayer��5�         GatesWpf.UIGates.Comment  5�      -   5        (  Name$( 	tbComment��5�   $   $	��Top=�67    . # [ 5�      $ 
gateCanvas��5�      $ adornerLayer��5�      5�      5�      9� 5�      -   5        $��expUserGates��5�   +   $��
User Gates��6   .��5�      �� -   5        $��spGates��5�   4   $ Vertical=�6   5�      5�      5�      5�      5�      