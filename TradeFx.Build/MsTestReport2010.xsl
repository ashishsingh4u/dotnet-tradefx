<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                version="1.0">
  <xsl:output method="html"/>

  <xsl:template match="/">
    <xsl:apply-templates select="/cruisecontrol/build/*[local-name()='TestRun']" />
  </xsl:template>

  <xsl:template match="/cruisecontrol/build/*[local-name()='TestRun']">
    <h1>
      Test Run: <xsl:value-of select="@name" />
    </h1>

    <h2>Summary</h2>
    <p>
      Designated outcome :
      <xsl:choose>
        <xsl:when test="*[local-name()='ResultSummary']/@outcome ='Passed'">
          <span style="color: forestGreen; font-weight: bold;">
            <xsl:value-of select="*[local-name()='ResultSummary']/@outcome" />
          </span>
        </xsl:when>
        <xsl:otherwise>
          <span style="color: Red; font-weight: bold;">
            <xsl:value-of select="*[local-name()='ResultSummary']/@outcome" />
          </span>
        </xsl:otherwise>
      </xsl:choose>
    </p>

    <table cellSpacing="0"
           cellPadding="4" >
      <thead style="text-align: center;">
        <td style="border-left: 1px solid #649cc0; border-bottom: 1px solid #649cc0; border-top: 1px solid #649cc0;">Number of Tests</td>
        <td style="background-color:#86ed60; color:black;border-left: 1px solid #649cc0; border-bottom: 1px solid #649cc0; border-top: 1px solid #649cc0;">Passed</td>
        <td style="background-color:#eb4848; color:black;border-left: 1px solid #649cc0; border-bottom: 1px solid #649cc0; border-top: 1px solid #649cc0;">Failed</td>
        <td style="background-color: yellow; color:black;border-left: 1px solid #649cc0; border-bottom: 1px solid #649cc0; border-top: 1px solid #649cc0;">Inconclusive</td>
        <td style="background-color: red; color: black;border-left: 1px solid #649cc0; border-bottom: 1px solid #649cc0; border-top: 1px solid #649cc0;">Aborted</td>
        <td style="background-color: darkblue; color: white;border-left: 1px solid #649cc0; border-bottom: 1px solid #649cc0; border-top: 1px solid #649cc0;">Timeout</td>
        <td style="background-color: darkblue; color: white;border-left: 1px solid #649cc0; border-bottom: 1px solid #649cc0; border-top: 1px solid #649cc0;">Started at</td>
        <td style="background-color: darkblue; color: white;border-left: 1px solid #649cc0; border-bottom: 1px solid #649cc0; border-top: 1px solid #649cc0;">Stopped at</td>
      </thead>
      <tr style="text-align: center;">
        <td style="border-left: 1px solid #649cc0; border-bottom: 1px solid #649cc0;">
          <xsl:value-of select="*[local-name()='ResultSummary']//*[local-name()='Counters']/@total" />
        </td>
        <td style="border-left: 1px solid #649cc0; border-bottom: 1px solid #649cc0;">
          <xsl:value-of select="*[local-name()='ResultSummary']//*[local-name()='Counters']/@passed" />
        </td>
        <td style="border-left: 1px solid #649cc0; border-bottom: 1px solid #649cc0;">
          <xsl:value-of select="*[local-name()='ResultSummary']//*[local-name()='Counters']/@failed" />
        </td>
        <td style="border-left: 1px solid #649cc0; border-bottom: 1px solid #649cc0;">
          <xsl:value-of select="*[local-name()='ResultSummary']//*[local-name()='Counters']/@inconclusive" />
        </td>
        <td style="border-left: 1px solid #649cc0; border-bottom: 1px solid #649cc0;">
          <xsl:value-of select="*[local-name()='ResultSummary']//*[local-name()='Counters']/@aborted" />
        </td>
        <td style="border-left: 1px solid #649cc0; border-bottom: 1px solid #649cc0;">
          <xsl:value-of select="*[local-name()='ResultSummary']//*[local-name()='Counters']/@timeout" />
        </td>
        <td style="border-left: 1px solid #649cc0; border-bottom: 1px solid #649cc0;">
          <xsl:call-template name="formatDateTime">
            <xsl:with-param name="dateTime"
                            select="*[local-name()='Times']/@creation" />
          </xsl:call-template>
        </td>
        <td style="border-left: 1px solid #649cc0; border-bottom: 1px solid #649cc0;; border-right: 1px solid #649cc0;">
          <xsl:call-template name="formatDateTime">
            <xsl:with-param name="dateTime"
                            select="*[local-name()='Times']/@finish" />
          </xsl:call-template>
        </td>
      </tr>
    </table>

    <xsl:variable name="runinfos"
                  select="*[local-name()='ResultSummary']/*[local-name()='RunInfos']/*[local-name()='RunInfo']" />
    <xsl:if test="count($runinfos) > 0">
      <h3>Errors and Warnings</h3>
      <table width="100%"
             cellSpacing="0"
             style="font-size:small;">
        <xsl:apply-templates select="$runinfos" />
      </table>
    </xsl:if>


    <xsl:apply-templates select="*[local-name()='Results']">
    </xsl:apply-templates>

  </xsl:template>

  <xsl:template match="*[local-name()='RunInfo']">
    <tr>
      <td>
        <pre>
          <xsl:apply-templates select="*" />
        </pre>
      </td>
    </tr>
  </xsl:template>

  <xsl:template match="*[local-name()='Results']">
    <h2>Test Results</h2>
    <table cellPadding="4"
           cellSpacing="0"
           width="98%">
      <thead style="border-top: 1px solid #649cc0; border-bottom: 1px solid #649cc0; border-right: 1px solid #649cc0;background-color: #a9d9f7; font-weight: bold;">
        <td style="text-align: center;border-left: 1px solid #649cc0; border-bottom: 1px solid #649cc0; border-top: 1px solid #649cc0;">Test List Name</td>
        <td style="text-align: center;border-left: 1px solid #649cc0; border-bottom: 1px solid #649cc0; border-top: 1px solid #649cc0;">Test Name</td>
        <td style="text-align: center;border-left: 1px solid #649cc0; border-bottom: 1px solid #649cc0; border-top: 1px solid #649cc0;">Test Result</td>
        <td style="text-align: center;border-left: 1px solid #649cc0; border-bottom: 1px solid #649cc0; border-top: 1px solid #649cc0;">Test Duration</td>
        <!--	   
	   <td>Test Start</td>
       <td>Test End</td>
-->
        <td style="text-align: center; border-right: 1px solid #649cc0; border-left: 1px solid #649cc0; border-bottom: 1px solid #649cc0; border-top: 1px solid #649cc0;">Class Name</td>
      </thead>
      <tr>
        <xsl:apply-templates select="./*" />
      </tr>
    </table>
  </xsl:template>

  <xsl:template match="*[local-name()='TestResultAggregation']">
    <tr>
      <td colspan="3">
        <center>
          <b>
            Composite Test : <xsl:value-of select="@testName "/>
          </b>
          <br />
          <table border="1"
                 style="text-align: center;"
                 cellSpacing="0"
                 cellpadding="5" >
            <thead>
              <td style="background-color:#86ed60; color:black">Passed</td>
              <td style="background-color:#eb4848; color:black;">Failed</td>
              <td style="background-color: yellow; color:black;">Inconclusive</td>
              <td style="background-color: red; color: black;">Aborted</td>
              <td style="background-color: darkblue; color:white;">Timeout</td>
            </thead>
            <tr>
              <td>
                <xsl:value-of select="*[local-name()='Counters']/@passed"/>
              </td>
              <td>
                <xsl:value-of select="*[local-name()='Counters']/@failed"/>
              </td>
              <td>
                <xsl:value-of select="*[local-name()='Counters']/@inconclusive"/>
              </td>
              <td>
                <xsl:value-of select="*[local-name()='Counters']/@aborted"/>
              </td>
              <td>
                <xsl:value-of select="*[local-name()='Counters']/@timeout"/>
              </td>
            </tr>
          </table>

        </center>
      </td>
    </tr>
    <xsl:apply-templates select="./*[local-name()='InnerResults']/*" />

    <tr>
      <td colspan="4"
          style="text-align: center; font-weight: bold;">
        End of composite test :  <xsl:value-of select="@testName "/>
      </td>
    </tr>
  </xsl:template>

  <xsl:template match="*[local-name()='UnitTestResult']">
    <xsl:variable name="tstid"
                  select="@testListId" />
    <xsl:variable name="tstListName"
                  select="/cruisecontrol/build/*[local-name()='TestRun']/*[local-name()='TestLists']/*[local-name()='TestList'][@id=$tstid]/@name" />
    <xsl:variable name="testid"
                  select="@testId" />
    <xsl:variable name="tstClassname"
                  select="/cruisecontrol/build/*[local-name()='TestRun']/*[local-name()='TestDefinitions']/*[local-name()='UnitTest'][@id=$testid]/*[local-name()='TestMethod']/@className" />
    <xsl:variable name="cn"
                  select="substring-before($tstClassname,',')" />

    <tr>
      <td style="text-align: center;border-left: 1px solid #649cc0; border-bottom: 1px solid #649cc0;">
        <xsl:value-of select="$tstListName"/>
      </td>
      <td style="text-align: center;border-left: 1px solid #649cc0; border-bottom: 1px solid #649cc0;">
        <xsl:value-of select="@testName"/>
      </td>
      <xsl:choose>
        <xsl:when test="@outcome = 'Passed'">
          <td style="text-align: center; font-weight: bold; background-color:#86ed60; color: black;border-left: 1px solid #649cc0; border-bottom: 1px solid #649cc0;">
            <xsl:value-of select="@outcome"/>
          </td>
        </xsl:when>
        <xsl:when test="@outcome = 'Failed'">
          <td style="text-align: center; font-weight: bold; background-color:#eb4848; color: black;border-left: 1px solid #649cc0; border-bottom: 1px solid #649cc0;">
            <xsl:value-of select="@outcome"/>
          </td>
        </xsl:when>
        <xsl:when test="@outcome = 'Inconclusive'">
          <td style="text-align: center; font-weight: bold; background-color: yellow; color: black;border-left: 1px solid #649cc0; border-bottom: 1px solid #649cc0;">
            <xsl:value-of select="@outcome"/>
          </td>
        </xsl:when>
        <xsl:otherwise>
          <td style="text-align: center; font-weight: bold; background-color: lightblue; color: black;border-left: 1px solid #649cc0; border-bottom: 1px solid #649cc0; ">
            <xsl:value-of select="@outcome"/>
          </td>
        </xsl:otherwise>
      </xsl:choose>
      <td style="text-align: right;border-left: 1px solid #649cc0; border-bottom: 1px solid #649cc0;">
        <xsl:value-of select="@duration"/>
      </td>
      <!--
     <td style="text-align: right;">
       <xsl:value-of select="@startTime"/>
     </td>

     <td style="text-align: right;">
       <xsl:value-of select="@endTime"/>
     </td>
-->
      <td style="text-align: left;border-left: 1px solid #649cc0; border-bottom: 1px solid #649cc0; border-right: 1px solid #649cc0;">
        <xsl:value-of select="$tstClassname"/>
      </td>

    </tr>
    <xsl:apply-templates select="./*[local-name()='Output']/*[local-name()='ErrorInfo']" />
  </xsl:template>

  <xsl:template match="*[local-name()='ErrorInfo']">
    <tr>
      <td colspan="5"
          bgcolor="#FF9900">
        <b>
          <xsl:value-of select="./*[local-name()='Message']" />
        </b>
        <br />
        <xsl:value-of select="./*[local-name()='StackTrace']" />
      </td>
    </tr>
  </xsl:template>

  <xsl:template match="*[local-name()='TestResult']">
    <xsl:variable name="tstid"
                  select="@testListId" />
    <xsl:variable name="tstListName"
                  select="/cruisecontrol/build/*[local-name()='TestRun']/*[local-name()='TestLists']/*[local-name()='TestList'][@id=$tstid]/@name" />
    <tr>
      <td>
        <xsl:value-of select="$tstListName"/>
      </td>
      <td>
        <xsl:value-of select="@testName"/>
      </td>
      <xsl:choose>
        <xsl:when test="@outcome = 'Passed'">
          <td style="text-align: center; font-weight: bold; background-color:#86ed60; color: black">
            <xsl:value-of select="@outcome"/>
          </td>
        </xsl:when>
        <xsl:when test="@outcome = 'Failed'">
          <td style="text-align: center; font-weight: bold; background-color:#eb4848; color: black  ">
            <xsl:value-of select="@outcome"/>
          </td>
        </xsl:when>
        <xsl:when test="@outcome = 'Inconclusive'">
          <td style="text-align: center; font-weight: bold; background-color: yellow; color: black;">
            <xsl:value-of select="@outcome"/>
          </td>
        </xsl:when>
        <xsl:otherwise>
          <td style="text-align: center; font-weight: bold; background-color: lightblue; color: black; ">
            <xsl:value-of select="@outcome"/>
          </td>
        </xsl:otherwise>
      </xsl:choose>
      <td style="text-align: right;">
        <xsl:value-of select="@duration"/>
      </td>
    </tr>
  </xsl:template>


  <xsl:template name="formatDateTime">
    <xsl:param name="dateTime" />
    <xsl:variable name="date"
                  select="substring-before($dateTime, 'T')" />
    <xsl:variable name="year"
                  select="substring-before($date, '-')" />
    <xsl:variable name="month"
                  select="substring-before(substring-after($date, '-'), '-')" />
    <xsl:variable name="day"
                  select="substring-after(substring-after($date, '-'), '-')" />

    <xsl:variable name="alltime"
                  select="substring-after($dateTime, 'T')" />
    <xsl:variable name="time"
                  select="substring-before($alltime, '.')" />

    <xsl:value-of select="concat($day, '-', $month, '-', $year, ' ',$time)" />
  </xsl:template>

</xsl:stylesheet>
