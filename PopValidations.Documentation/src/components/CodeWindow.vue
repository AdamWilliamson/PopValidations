<script lang="ts">
import Prism from "prismjs";
import "prismjs/components/prism-csharp";
import "prismjs/components/prism-json";

export default {
  props: {
    source: {
      type: String,
      default: "console.log('PrismJs');",
    },
    language: {
      type: String,
      default: "csharp",
    },
    copy: {
      type: String,
      default: "Copy code",
    },
    copiedTip: {
      type: String,
      default: "Copied!",
    },
  },
  data() {
    var prismHTML = "";
    var loaded = false;

    return {
      prismHTML,
      loaded,
      
    };
  },
  async mounted() {
    const lineNumbersDiv = document.createElement("div");
    lineNumbersDiv.setAttribute("class", "line-numbers-span");
    this.$refs.preNode.append(lineNumbersDiv);
    lineNumbersDiv.style.position = "absolute";
    lineNumbersDiv.style.top = "1.5em";
    lineNumbersDiv.style.left = "0em";
    lineNumbersDiv.style.width = "3em";
    lineNumbersDiv.style.borderRight = "1px solid #999";
    lineNumbersDiv.style.textAlign = "center";

    this.source
      .trim()
      .split("\n")
      .forEach((_, index) => {
        let span = document.createElement("span");
        let br = document.createElement("br");
        br.style.userSelect = "none";
        span.innerHTML = "" + index + 1;
        span.style.color = "#999";
        lineNumbersDiv.append(span);
        lineNumbersDiv.append(br);
      });

    const copySpan = document.createElement("span");
    copySpan.setAttribute("class", "copy-span");
    this.$refs.preNode.append(copySpan);
    copySpan.innerHTML = this.copy;
    copySpan.style.position = "absolute";
    copySpan.style.top = "0.3em";
    copySpan.style.right = "0em";
    copySpan.style.padding = "0 0.5em";
    copySpan.style.cursor = "pointer";
    copySpan.style.borderRadius = "0.5em";
    copySpan.style.color = "#bbb";
    copySpan.style.backgroundColor = "rgba(224, 224, 224, 0.2)";
    copySpan.style.boxShadow = "0 2px 0 0 rgb(0 0 0 / 20%)";
    copySpan.style.opacity = "0";

    copySpan.onclick = () => {
      copySpan.innerHTML = this.copiedTip;
      if (navigator.clipboard) {
        navigator.clipboard.writeText(this.source);
      } else {
        const textarea = document.createElement("textarea");
        document.body.appendChild(textarea);
        textarea.style.position = "fixed";
        textarea.style.clipPath = "circle(0)";
        textarea.style.top = "10px";
        textarea.value = this.source.trim();
        textarea.select();
        document.execCommand("copy", true);
        document.body.removeChild(textarea);
      }
      setTimeout(() => {
        copySpan.innerHTML = this.copy;
      }, 1000);
    };

    this.$refs.preNode.onmouseover = () => {
      copySpan.style.opacity = "1";
    };

    this.$refs.preNode.onmouseout = () => {
      copySpan.style.opacity = "0";
    };

    this.prismHTML = Prism.highlight(
      `${this.source.trim()}`,
      Prism.languages[this.language],
      this.language
    );
  },
};
</script>

<template>
  <div class="theme" style="width:100%;height:100%;">
    <pre
      ref="preNode"
      :class="'language-' + language"
    ><code :class="'language-'+language" v-html="prismHTML"></code></pre>
  </div>
</template>


<style lang="scss" scoped>
pre {
  margin: 1rem auto;
  width: fit-content;
  box-sizing: border-box;
  max-width: 90%;
  padding-left: 3.8em !important;
  padding-top: 1.5em !important;
  padding-bottom: 2em;
  position: relative;
}
</style>

<style lang="scss">
.theme {
    background: #2f2f2f;
//https://github.com/PrismJS/prism-themes/blob/master/themes/prism-material-dark.css
code[class*="language-"],
pre[class*="language-"] {
	text-align: left;
	white-space: pre;
	word-spacing: normal;
	word-break: normal;
	word-wrap: normal;
	color: #eee;
	background: #2f2f2f;
	font-family: Roboto Mono, monospace;
	font-size: 1em;
	line-height: 1.5em;

	-moz-tab-size: 4;
	-o-tab-size: 4;
	tab-size: 4;

	-webkit-hyphens: none;
	-moz-hyphens: none;
	-ms-hyphens: none;
	hyphens: none;
}

code[class*="language-"]::-moz-selection,
pre[class*="language-"]::-moz-selection,
code[class*="language-"] ::-moz-selection,
pre[class*="language-"] ::-moz-selection {
	background: #363636;
}

code[class*="language-"]::selection,
pre[class*="language-"]::selection,
code[class*="language-"] ::selection,
pre[class*="language-"] ::selection {
	background: #363636;
}

:not(pre) > code[class*="language-"] {
	white-space: normal;
	border-radius: 0.2em;
	padding: 0.1em;
}

pre[class*="language-"] {
	overflow: auto;
	position: relative;
	margin: 0.5em 0;
	padding: 1.25em 1em;
}

.language-css > code,
.language-sass > code,
.language-scss > code {
	color: #fd9170;
}

[class*="language-"] .namespace {
	opacity: 0.7;
}

.token.atrule {
	color: #c792ea;
}

.token.attr-name {
	color: #ffcb6b;
}

.token.attr-value {
	color: #a5e844;
}

.token.attribute {
	color: #a5e844;
}

.token.boolean {
	color: #c792ea;
}

.token.builtin {
	color: #ffcb6b;
}

.token.cdata {
	color: #80cbc4;
}

.token.char {
	color: #80cbc4;
}

.token.class {
	color: #ffcb6b;
}

.token.class-name {
	color: #f2ff00;
}

.token.comment {
	color: #616161;
}

.token.constant {
	color: #c792ea;
}

.token.deleted {
	color: #ff6666;
}

.token.doctype {
	color: #616161;
}

.token.entity {
	color: #ff6666;
}

.token.function {
	color: #c792ea;
}

.token.hexcode {
	color: #f2ff00;
}

.token.id {
	color: #c792ea;
	font-weight: bold;
}

.token.important {
	color: #c792ea;
	font-weight: bold;
}

.token.inserted {
	color: #80cbc4;
}

.token.keyword {
	color: #c792ea;
}

.token.number {
	color: #fd9170;
}

.token.operator {
	color: #89ddff;
}

.token.prolog {
	color: #616161;
}

.token.property {
	color: #80cbc4;
}

.token.pseudo-class {
	color: #a5e844;
}

.token.pseudo-element {
	color: #a5e844;
}

.token.punctuation {
	color: #89ddff;
}

.token.regex {
	color: #f2ff00;
}

.token.selector {
	color: #ff6666;
}

.token.string {
	color: #a5e844;
}

.token.symbol {
	color: #c792ea;
}

.token.tag {
	color: #ff6666;
}

.token.unit {
	color: #fd9170;
}

.token.url {
	color: #ff6666;
}

.token.variable {
	color: #ff6666;
}


//   /**
//  * a11y-dark theme for JavaScript, CSS, and HTML
//  * Based on the okaidia theme: https://github.com/PrismJS/prism/blob/gh-pages/themes/prism-okaidia.css
//  * @author ericwbailey
//  */
//   /* https://github.com/PrismJS/prism-themes/blob/master/themes/prism-a11y-dark.css */

//   code[class*="language-"],
//   pre[class*="language-"] {
//     color: #f8f8f2;
//     background: none;
//     font-family: Consolas, Monaco, "Andale Mono", "Ubuntu Mono", monospace;
//     text-align: left;
//     white-space: pre;
//     word-spacing: normal;
//     word-break: normal;
//     word-wrap: normal;
//     line-height: 1.5;
//     text-shadow: 0 0;

//     -moz-tab-size: 4;
//     -o-tab-size: 4;
//     tab-size: 4;

//     -webkit-hyphens: none;
//     -moz-hyphens: none;
//     -ms-hyphens: none;
//     hyphens: none;
//     ::selection{
//         background:      slategray;
//     }
//   }

//   /* Code blocks */
//   pre[class*="language-"] {
//     padding: 1em;
//     margin: 0.5em 0;
//     overflow: auto;
//     border-radius: 0.3em;
//   }

//   :not(pre) > code[class*="language-"],
//   pre[class*="language-"] {
//     background: #2b2b2b;
//   }

//   /* Inline code */
//   :not(pre) > code[class*="language-"] {
//     padding: 0.1em;
//     border-radius: 0.3em;
//     white-space: normal;
//   }

//   .token.comment,
//   .token.prolog,
//   .token.doctype,
//   .token.cdata {
//     color: #d4d0ab;
//   }

//   .token.punctuation {
//     color: #fefefe;
//   }

//   .token.property,
//   .token.tag,
//   .token.constant,
//   .token.symbol,
//   .token.deleted {
//     color: #ffa07a;
//   }

//   .token.boolean,
//   .token.number {
//     color: #00e0e0;
//   }

//   .token.selector,
//   .token.attr-name,
//   .token.string,
//   .token.char,
//   .token.builtin,
//   .token.inserted {
//     color: #abe338;
//   }

//   .token.operator,
//   .token.entity,
//   .token.url,
//   .language-css .token.string,
//   .style .token.string,
//   .token.variable {
//     color: #00e0e0;
//     background: none !important;
//     ::selection{
//         background: none !important;
//     }
//   }

//   .token.atrule,
//   .token.attr-value,
//   .token.function {
//     color: #ffd700;
//   }

//   .token.keyword {
//     color: #00e0e0;
//   }

//   .token.regex,
//   .token.important {
//     color: #ffd700;
//   }

//   .token.important,
//   .token.bold {
//     font-weight: bold;
//   }

//   .token.italic {
//     font-style: italic;
//   }

//   .token.entity {
//     cursor: help;
//   }
}
</style>