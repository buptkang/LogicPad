ExpressionUserControl

DiagramUserControl

TruthTableUserControl

==========================================

Parsing those three controls

A + BC

O = (A OR (B AND C)) 

----------------

O = (A OR ( NOT B))

InterTree: Intermediate UserControls Representation


Diagram => XElement => InterTree => String

Expression => starPadSDK.MathExpr.Expr => XElement => InterTree => String

TruthTable => ???