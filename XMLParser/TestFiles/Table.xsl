<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    
<xsl:template match="/">
    
    <html>
    <body>
        <table border="1">
            <tr>
                <xsl:for-each select="List/Label">
                    <td><b><xsl:value-of select="." /></b></td>
                </xsl:for-each>
            </tr>
            
            <xsl:for-each select="List/Scientist">
                <tr>
                    <td><xsl:value-of select="FullName" /></td>
                    <td><xsl:value-of select="Faculty" /></td>
                    <td><xsl:value-of select="Department" /></td>
                    <td><xsl:value-of select="ScientificDegree" /></td>
                    <td><table>
                        <xsl:for-each select="AcademicTitles">
                            <tr><td><xsl:value-of select="." /></td></tr>
                        </xsl:for-each>
                    </table></td>
                </tr>
            </xsl:for-each>
            
        </table>
    </body>
    </html>
    
</xsl:template>
    
</xsl:stylesheet>