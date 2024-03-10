<script setup lang="ts">

import {CSSProperties, readonly} from 'vue'
import { ref } from 'vue'
import {basicSetup } from 'codemirror'
import { Codemirror } from 'vue-codemirror'
import { vue } from '@codemirror/lang-vue'
import { oneDark } from '@codemirror/theme-one-dark'
const cm = ref(null)

interface Props {
  codeStyle?: CSSProperties // 代码样式
  dark?: boolean // 是否暗黑主题
  code?: string // 代码字符串
  // placeholder?: string // 占位文本
  // autofocus?: boolean // 自动聚焦
  // disabled?: boolean // 禁用输入行为和更改状态
  // indentWithTab?: boolean // 启用tab按键
  // tabSize?: number // tab按键缩进空格数
}
const props = withDefaults(defineProps<Props>(), {
  // placeholder: 'Code goes here...',
  codeStyle: () => { return {} },
  dark: false,
  code: '',

})
const cmOptions = readonly({
  lineNumbers: false,
})
console.log('basicSetup: ', basicSetup)
const extensions = props.dark ? [vue(), oneDark, basicSetup] : [vue(), basicSetup]
const codeValue = ref(props.code)

</script>

<template>
  <Codemirror
      disabled
      ref="cm"
      v-model="codeValue"
      :style="codeStyle"
      :option="cmOptions"
      :extensions="extensions"
      v-bind="$attrs"
  />
</template>

<style scoped>

</style>